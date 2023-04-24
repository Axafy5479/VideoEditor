using CommandProject.Commands;
using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;

namespace ParameterEditor.Commands
{
    internal class Command_ZoomChange : ScreenCommandBase<IRenderingItem>
    {
        public Command_ZoomChange(ReadOnlyCollection<IRenderingItem> selectedItem, int zoom) : base(selectedItem)
        {
            foreach (var item in selectedItem)
            {
                OldZooms.Add(item.Zoom);
            }
            NewZoom = zoom;
        }

        private List<double> OldZooms { get; } = new();
        private double NewZoom { get; }

        protected override ErrorInfo? _Redo()
        {
            return _Execute();
        }

        protected override ErrorInfo? _Undo()
        {
            for (int i = 0; i < N; i++)
            {
               // SelectedItem[i].Change_Zoom(OldZooms[i]);
                SelectedItem[i].Zoom = OldZooms[i];
            }
            return null;
        }

        protected override ErrorInfo? _Execute()
        {
            foreach (var item in SelectedItem)
            {
                //item.Change_Zoom(NewZoom);
                item.Zoom = NewZoom;
            }

            return null;
        }
    }
}
