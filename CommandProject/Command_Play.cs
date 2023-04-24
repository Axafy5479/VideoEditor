using System;
using System.Collections.Generic;
using System.Linq;
using CommandProject.Commands;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Timeline.Error;
using Data;

namespace Timeline.Commands
{
    internal class Command_Play : CommandBase
    {
        public override bool CanExecute(object? parameter) => true;

        protected override ErrorInfo? _Execute()
        {
            TimelineDataController.Instance.Playing.Value = !TimelineDataController.Instance.Playing.Value;
            return null;
        }
    }
}
