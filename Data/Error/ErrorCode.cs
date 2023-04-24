using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timeline.Error
{
    public enum WarningCode
    {
        [ErrorMessage("おぷじぇくとプールの容量を変更しました。")]
        PoolCapacityChanged,
    }

    public enum ErrorCode
    {
        [ErrorMessage("原因が不明なエラーが生じました")]
        UnknownError,

        [ErrorMessage("CommandのExecuteが実行される前にUndo又はRedoが実行されたため、CommandInfoのインスタンスが生成されていません")]
        CommandInfoNull,

        [ErrorMessage("同一のCommandインスタンスのExecuteが2度実行されました。")]
        CommandExecutedTwice,

        [ErrorMessage("TLItemを配置すべきレイヤーがnullです")]
        LayerForItemIsNull,

        [ErrorMessage("TLItemObjectの親レイヤーがTimelineBaseObjectではありません")]
        LayerForItemIsNotTimelineBaseObject,


        [ErrorMessage("CursorTargetの親オブジェクトがTimelineItemObjectではありません")]
        ParentOfCursorTargetIsNotTLItemObject,

        [ErrorMessage("TLItemのbyteデータからDeserializingに失敗しました")]
        DeserializingFailed,

        [ErrorMessage("TLItemからbyteデータへのSerializingに失敗しました")]
        SerializingFailed,

        [ErrorMessage("削除対象のTimelineObjectが見つかりませんでした")]
        RemovingItemNotFound,

        [ErrorMessage("指定された位置にTLItemObjectは存在しませんでした")]
        TimelineItemObjectNotFoundAtSpecificPosition,

        [ErrorMessage("指定された位置にTLItemは存在しませんでした")]
        TimelineItemNotFoundAtSpecificPosition,

        [ErrorMessage("コマンドの対象となるTLItemの数がゼロです")]
        NoCommandTarget,

        [ErrorMessage("TLItemが存在するレイヤーの削除は出来ません")]
        CantDeleteLayerWithTLItem,

        [ErrorMessage("このかくちょしを持つファイルは使用できません")]
        InputFileExtentionCantBeUsed,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorMessageAttribute : Attribute
    {
        public ErrorMessageAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public static class ErrorUtil
    {
        public static string ToMessage(this ErrorCode error)
        {
            var type = typeof(ErrorCode);
            var lookup = type.GetFields()
              .Where(x => x.FieldType == type)
              .SelectMany(x => x.GetCustomAttributes(false), (x, attribute) => new { code = x.GetValue(null), attribute })
              .ToLookup(x => x.attribute.GetType());

            var errorMessageAttributes = lookup[typeof(ErrorMessageAttribute)].ToDictionary(x => x.code, x => (ErrorMessageAttribute)x.attribute);

            if (errorMessageAttributes.TryGetValue(error, out var errorMessageAttribute))
            {
                return errorMessageAttribute.Message;
            }
            else
            {
                return "";
            }
        }

    }

    public class ErrorInfo
    {
        public ErrorInfo(ErrorCode errorCode, Exception? exception = null, string userMessage = "")
        {
            Debug.WriteLine(errorCode);
            ErrorCode = errorCode;
            Message = errorCode.ToMessage();
            StackTrace = Environment.StackTrace;
            Exception = exception;
            UserMessage = userMessage;
        }

        public ErrorCode ErrorCode { get; }
        public string Message { get; }
        public string StackTrace { get; }
        public Exception? Exception { get; }
        public string UserMessage { get; }
    }

}
