using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timeline.Error
{
    public class ErrorHandler
    {
        private static ErrorHandler instance;
        public static ErrorHandler Instance => instance ??= new();
        public static bool SafeMode = false;

        private ErrorHandler() { }

        public List<ErrorInfo> ErrorLog { get; } = new();
        public List<string> ErrorMessage { get; } = new();

        public void Add(ErrorCode errorCode)
        {
            Add(new ErrorInfo(errorCode));
        }

        public void Add(ErrorInfo? _errorInfo)
        {
            ErrorInfo errorInfo = _errorInfo ?? new(ErrorCode.UnknownError);

            ErrorLog.Add(errorInfo);
            if(!SafeMode)
            {
                throw new Exception($"ハンドルされた例外{errorInfo.ErrorCode}が発生しました\nmessage={errorInfo.Message}\nuserMessage={errorInfo.UserMessage}");
            }
        }

        public bool HasError => ErrorLog.Any();
    }
}
