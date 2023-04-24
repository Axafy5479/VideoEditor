using Data;
using MessagePack;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using CommandProject.Commands;
using System.Windows;
using System.Windows.Input;
using Timeline.Commands;
using Timeline.CustomControl;
using CommandProject;
using YukkuriCharacterSettingsProject.TLWindow;
using System.Reactive.Linq;

namespace Timeline
{
    public class TimelineBaseViewModel
    {
        public TimelineBaseViewModel()
        {
            //OnContextMenuSelected.Subscribe(Execute);
        }

        //private Subject<Type> OnContextMenuSelected { get; } = new();


        public void AddCommand(TimelineKeyboard keyboardListener)
        {
            keyboardListener.OnCommandKeyChanged.Subscribe(Execute);
            keyboardListener.OnAltCtrlCommand.Subscribe(
                b =>
                {
                    if (b && !YukkuriTLItemMaker.IsShowing)
                    {
                        new YukkuriTLItemMaker().Show();
                    }
                    else if (!b && YukkuriTLItemMaker.IsShowing)
                    {
                        YukkuriTLItemMaker.CloseWin();
                    }
                });

        }

        private void Execute(Hotkey hotkeyarg)
        {
            Debug.WriteLine(hotkeyarg);


            if (hotkeyarg.Modif == ModifierKeys.Control && hotkeyarg.Key == Key.Z)
            {
                CommandInvoker.Instance.Undo();
                return;
            }
            else if (hotkeyarg.Modif == ModifierKeys.Control && hotkeyarg.Key == Key.Y)
            {
                CommandInvoker.Instance.Redo();
                return;
            }

            var hotkey = new Hotkey(hotkeyarg.Modif, hotkeyarg.Key);
            CommandProject.CommandManager.Instance.Execute(hotkey);
        }



    }

}



