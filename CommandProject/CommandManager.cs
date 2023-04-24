using CommandProject.Commands;
using CommunityToolkit.Mvvm.Input;
using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Timeline.Commands;

namespace CommandProject
{
    public class CommandManager
    {
        private static CommandManager? instance;
        public static CommandManager Instance => instance ??= new();

        public void ContextMenuOpening()
        {
            foreach (var item in ContextMenu)
            {
                item.Command.NotifyCanExecuteChanged();
            }
        }


        private CommandManager()
        {
            foreach (var command in TypeCommandMap)
            {
                if (command.Value.ShowInContextMenu)
                {
                    ContextMenu.Add(new(command.Value));
                }

            }
        }

        public List<MenuViewModel> ContextMenu { get; } = new();

        public static readonly Dictionary<Type, CommandInfo> TypeCommandMap = new()
        {
            {typeof(Command_Move), new("TLアイテムの移動", ()=>new Command_Move(), false) },
            {typeof(Command_Remove), new("削除", ()=>new Command_Remove(), true, new Hotkey(ModifierKeys.None,Key.Delete)) },
            {typeof(Command_RemoveAndFillBlank), new("削除して間隙を詰める", () => new Command_RemoveAndFillBlank(), true, new Hotkey(ModifierKeys.Shift,Key.Delete)) },
            {typeof(Command_Divide), new("TLアイテムの分割", () => new Command_Divide(), true, new Hotkey(ModifierKeys.Control,Key.K)) },
            {typeof(Command_Copy), new("コピー", () => new Command_Copy(), true, new Hotkey(ModifierKeys.Control,Key.C)) },
            {typeof(Command_Cut), new("切り取り", () => new Command_Cut(), true, new Hotkey(ModifierKeys.Control,Key.X)) },
            {typeof(Command_Paste), new("ペースト", () => new Command_Paste(), true, new Hotkey(ModifierKeys.Control,Key.V)) },
            //{typeof(Command_InsertLayer), new("新しいレイヤーの挿入", ()=>new Command_InsertLayer()) },
            {typeof(Command_Play), new("再生", () => new Command_Play(), true, new Hotkey(ModifierKeys.Control,Key.Space)) },

        };

        public void Execute<T>() where T : CommandBase => Execute(typeof(T));
        public void Execute(Type type)
        {
            CommandInvoker.Instance.Execute(TypeCommandMap[type].GenerateCommand());
        }

        public void Execute(Hotkey hotkey)
        {
            var foundCommand = TypeCommandMap.Values.Find(c=>c.Hotkey.Value.Equals(hotkey));
            if (foundCommand != null)
            {
                CommandInvoker.Instance.Execute(foundCommand.GenerateCommand());
            }
        }
    }

    public class MenuViewModel
    {
        public MenuViewModel(CommandInfo commandInfo)
        {
            MenuText = commandInfo.MenuText;
            ShowInContextMenu = commandInfo.ShowInContextMenu;
            Command = commandInfo.GenerateCommand().CreateContextmenuCommand();
            Hotkey = commandInfo.Hotkey;
        }

        public string MenuText { get; }
        public bool ShowInContextMenu { get; }
        public RelayCommand Command { get; }
        public IReadOnlyReactiveProperty<Hotkey> Hotkey { get; }
    }
    public class CommandInfo
    {
        public CommandInfo(string menuText, Func<CommandBase> generateCommand, bool showInContextMenu, Hotkey? hotkey = null)
        {
            MenuText = menuText;
            ShowInContextMenu = showInContextMenu;
            GenerateCommand = generateCommand;
            this.hotkey.Value = hotkey ?? new(ModifierKeys.None, Key.None);
        }

        public string MenuText { get; }
        public bool ShowInContextMenu { get; }
        public Func<CommandBase> GenerateCommand { get; }
        private ReactiveProperty<Hotkey> hotkey = new();
        public IReadOnlyReactiveProperty<Hotkey> Hotkey => hotkey;
    }

    public struct Hotkey
    {
        public Hotkey(ModifierKeys modif, Key key)
        {
            Modif = modif;
            Key = key;
        }

        public ModifierKeys Modif { get; }
        public Key Key { get; }

        public static bool operator ==(Hotkey left, Hotkey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Hotkey left, Hotkey right)
        {
            return !left.Equals(right);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Hotkey h) return false;
            else
            {
                return h.Key == Key && h.Modif == Modif;
            }
        }

        public override string ToString()
        {
            if(Key == Key.None)
            {
                ModifierKeysConverter converter = new ModifierKeysConverter();
                return converter.ConvertToString(Modif);
            }
            else if(Modif == ModifierKeys.None)
            {
                return Key.ToString();
            }
            else
            {
                ModifierKeysConverter converter = new ModifierKeysConverter();
                return converter.ConvertToString(Modif) + "+" + Key;
            }
        }
    }
}
