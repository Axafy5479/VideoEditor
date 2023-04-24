using CommandProject.Commands;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;

namespace CommandProject.Commands
{
    public interface IRedoable
    {
        public ErrorInfo? Redo();
        public ErrorInfo? Undo();
    }

    /// <summary>
    /// TimelineDataCoreの値を変更するコマンド
    /// </summary>
    public abstract class RedoableCommandBase : CommandBase, IRedoable
    {

        public ErrorInfo? Undo() => Error ?? _Undo();
        protected abstract ErrorInfo? _Undo();

        public ErrorInfo? Redo() => Error ?? _Redo();
        protected abstract ErrorInfo? _Redo();
    }

    /// <summary>
    /// TimelineDataCoreの値を変更するコマンド
    /// </summary>
    public abstract class RedoableCommandBase<T> : CommandBase<T>, IRedoable where T : IItemObjectViewModel
    {
        public ErrorInfo? Undo() => Error ?? _Undo();
        protected abstract ErrorInfo? _Undo();

        public ErrorInfo? Redo() => Error ?? _Redo();
        protected abstract ErrorInfo? _Redo();
    }
}
