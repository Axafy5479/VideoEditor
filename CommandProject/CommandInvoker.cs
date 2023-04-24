using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;

namespace CommandProject.Commands
{
    public class CommandInvoker
    {
        private CommandInvoker() { }
        private static CommandInvoker? instance;
        public static CommandInvoker Instance => instance ??= new();

        private List<IRedoable> history = new();
        private Stack<IRedoable> undoed = new();

        public void Execute(CommandBase command)
        {
            if (command.Execute() is ErrorInfo e)
            {
                ErrorHandler.Instance.Add(e);
            }
            else if(command is IRedoable redoable)
            {
                history.Add(redoable);
                undoed.Clear();
            }

        }

        public void Undo()
        {
            if(!history.Any())
            {
                return;
            }

            var command = history.Last();

            if (command.Undo() is ErrorInfo e)
            {
                ErrorHandler.Instance.Add(e);
                return;
            }

            history.RemoveAt(history.Count - 1);
            undoed.Push(command);
        }

        public void Redo()
        {
            if (!undoed.Any())
            {
                return;
            }

            var command = undoed.Peek();
            if(command.Redo() is ErrorInfo e)
            {
                ErrorHandler.Instance.Add(e);
                return;
            }
            undoed.Pop();
            history.Add(command);
        }
    }
}
