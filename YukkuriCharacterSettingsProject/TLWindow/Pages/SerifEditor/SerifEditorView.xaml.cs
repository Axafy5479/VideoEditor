using AquestalkProj;
using CommandProject.Commands;
using Data;
using MahApps.Metro.Controls;
using NAudioProj;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YukkuriCharacterSettingsProject.CustomControls;

namespace YukkuriCharacterSettingsProject.TLWindow
{
    /// <summary>
    /// SerifEditorView.xaml の相互作用ロジック
    /// </summary>
    public partial class SerifEditorView : UserControl
    {
        public SerifEditorView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var textBoxes = this.FindChildren<TextBox>();

                foreach (var item in textBoxes)
                {
                    if (item.Name == "SerifText")
                        SerifTextBox = item;
                    else if (item.Name == "PronuntiationText")
                        PronuntiationTextBox = item;
                }


                // Shift-Enterで改行
                this.InputBindings.Add(new KeyBinding(
                    new DelegateCommand(() => OnShiftEnterDown()), Key.Enter, ModifierKeys.Shift));

                // Enterで決定
                this.InputBindings.Add(new KeyBinding(
                    new DelegateCommand(() => OnEnterDown()), Key.Enter, ModifierKeys.None));

                SerifTextBox.TextChanged += (s, e) =>
                {
                    PronuntiationTextBox.Text = Kanji2Koe.Convert(SerifTextBox.Text);
                };

                Keyboard.Focus(SerifTextBox);
            };
        }

        public void Initialize(CharacterSettingManager setting)
        {
            ViewModel.Initialize(setting);
            Setting = setting;

        }

        public SerifEditorVM ViewModel => (SerifEditorVM)DataContext;
        public CharacterSettingManager Setting { get; private set; }

        public TextBox SerifTextBox { get; private set; }
        public TextBox PronuntiationTextBox { get; private set; }

        private void OnShiftEnterDown()
        {
            SerifTextBox.Text += Environment.NewLine;
            Keyboard.Focus(SerifTextBox);
            SerifTextBox.Select(SerifTextBox.Text.Length, 0);
        }

        private void OnEnterDown()
        {
            var timeSec = new Talker(PronuntiationText.Text, Setting.VoiceSettings.Pitch / 100, Setting.VoiceSettings.Speed / 100, Setting.VoiceSettings.VoiceType).AllLengthSec;
             int length = (int)Math.Round(timeSec * 60);

            var item = new VoiceItem(SerifTextBox.Text, PronuntiationText.Text, Setting, 0, Pointer.Instance.CursorFrame.Value, length);



            CommandInvoker.Instance.Execute(new Command_AddItem(new() { item }));

        }

        private void OnNewlineButtonClicked(object sender, RoutedEventArgs e)
        {
            OnShiftEnterDown();
        }

        private void OnDecideButtonClicked(object sender, RoutedEventArgs e)
        {
            OnEnterDown();
        }
    }
}
