using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandProject.Commands;
using Data;
using Timeline.Error;

namespace CommandProject.Commands
{
    public class Command_Copy : CommandBase<IItemObjectViewModel>
    {
        protected override ErrorInfo? _Execute()
        {
            return Pointer.Instance.Copy();
        }
    }
}
