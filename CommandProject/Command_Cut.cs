using Data;
using Timeline.Error;

namespace CommandProject.Commands
{
    public class Command_Cut : RedoableCommandBase<IItemObjectViewModel>
    {
        private Command_Remove RemoveCommand { get; set; }

        protected override ErrorInfo? _Redo() => _Execute();

        protected override ErrorInfo? _Undo() => RemoveCommand.Undo();

        protected override ErrorInfo? _Execute()
        {
            Error = new Command_Copy().Execute();
            if(Error is null) RemoveCommand = new Command_Remove();
            return RemoveCommand.Execute();
        }
    }
}
