using CommandProject;
using Reactive.Bindings;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timeline
{
    public class TimelineKeyboard : Grid
    {

        public TimelineKeyboard()
        {
            Loaded += (s, e) => OnLoad();
        }

        private bool loaded = false;
        private void OnLoad()
        {
            if (loaded) return;

            loaded = true;

            if (Application.Current != null && Application.Current.MainWindow!=null)
            {
                Application.Current.MainWindow.KeyDown += OnKeyDown;
                Application.Current.MainWindow.KeyUp += OnKeyUp;
            }
        }



        private Hotkey PrevHotkey { get; set; }

        public Subject<Hotkey> OnCommandKeyChanged { get; } = new();
        public Subject<bool> OnAltCtrlCommand { get; } = new();


        private void OnKeyDown(object sender, KeyEventArgs e)
        {

            // 現在押されている修飾キー全て
            var modifierKeys = Utility.CurrentModifierKeys();

            // 押されたキーが修飾キーか否か
            var keyTemp = e.Key == Key.System ? e.SystemKey : e.Key;
            var modifTemp = Utility.KeyToModif(keyTemp);
            var isModif = modifTemp != ModifierKeys.None;
            var key = isModif ? Key.None : keyTemp;

            Hotkey newHotkey = new Hotkey(modifierKeys, key);
            if(newHotkey == PrevHotkey)
            {
                // このメソッドはキーが押されている間常に呼ばれ続ける
                // 仮に前回のキーが同じであれば中身を実行せずに終了する
                return;
            }

            Debug.WriteLine(newHotkey);

            if (key != Key.None)
            {
                OnCommandKeyChanged.OnNext(newHotkey);
                e.Handled = true;
            }

            else if (newHotkey.Modif == (ModifierKeys.Alt | ModifierKeys.Control))
            {
                // 押されたキーが修飾キーの場合 Alt+Ctrl の時のみ通知
                OnAltCtrlCommand.OnNext(true);
                return;
            }

            PrevHotkey = newHotkey;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // 現在押されている修飾キー全て
            var modifierKeys = Utility.CurrentModifierKeys();

            // 押されたキーが修飾キーか否か
            var keyTemp = e.Key == Key.System ? e.SystemKey : e.Key;
            var modifTemp = Utility.KeyToModif(keyTemp);
            var isModif = modifTemp != ModifierKeys.None;
            var key = isModif ? keyTemp : Key.None;

            if (modifTemp == ModifierKeys.Alt || modifTemp== ModifierKeys.Control)
            {
                // 押されたキーが修飾キーの場合 Alt+Ctrl の時のみ通知
                OnAltCtrlCommand.OnNext(false);
            }

            PrevHotkey = new Hotkey(modifierKeys,key);
        }


    }
}
