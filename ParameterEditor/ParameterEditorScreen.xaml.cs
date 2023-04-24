using CommandProject.Commands;
using CommandProject.ScreenCommands;
using Data;
using ParameterEditor.ColorpickerWindow;
using ParameterEditor.CustomControls;
using ParameterEditor.CustomControls.Color;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
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
using TimeController;

namespace ParameterEditor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ParameterEditorScreen : UserControl
    {
        public ParameterEditorScreen()
        {
            InitializeComponent();

            Pointer.Instance.SelectedItem.Where(o => o is not null).Subscribe(OnChangeSelectedItem);

            DataContext = Parameters;

        }

        private ReactiveCollection<IDisposable> Parameters { get; } = new();
        public ReadOnlyCollection<ITimelineObjectViewModel> SelectedItems { get; private set; }



        private void OnChangeSelectedItem(List<ITimelineObjectViewModel> objs)
        {
            // !!!! Bug !!!!
            Debug.WriteLine("���O�ɍ폜�����A�C�e���̃p�����[�^�[���ҏW���g�p�Ƃ���");


            #region General
            foreach (var item in Parameters)
            {
                item.Dispose();
            }

            Parameters.Clear();


                
            if (!objs.Any()) return;

            var layer_p = new Parameter_Int(
                objs.ConvertAll(o=>o.Layer),
                0,
                TimelineDataController.Instance.MaxLayer.Value-1, _=>new Command_Move(),
                "���C���[")
            { Margin = new Thickness(0, 0, 0, 10) };

            var frame_p = new Parameter_Int(
            objs.ConvertAll(o => o.Frame),
            0,
            int.MaxValue, _ => new Command_Move(),
            "�J�n�t���[��")
            { Margin = new Thickness(0, 0, 0, 10) };


            var length_p = new Parameter_Int(
            objs.ConvertAll(o => o.Length),
            0,
            int.MaxValue, _ => new Command_Move(),
            "�t���[����")
            { Margin = new Thickness(0, 0, 0, 10) };

            ParameterSection p = new() { SectionTitle = "�^�C�����C��" };
            p.AddParameters(new() { layer_p, frame_p, length_p });
            Parameters.Add(p);
            #endregion


            #region RenderingObject
            var renderVM = objs.Select(o => o as IRenderingScreenObjVM).NonNull();

            if (renderVM.Any()) {

                var zoom_p = new Parameter_Double(
                    renderVM.ConvertAll(o => o.Zoom),
                    0,
                    int.MaxValue,
                    _ => new Command_Zoom(),
                    "�g�嗦", 0.1)
                { Margin=new Thickness(0,0,0,10) };

                var pos_p = new Parameter_DoubleDouble(
                    renderVM.ConvertAll(o => o.X),
                    renderVM.ConvertAll(o => o.Y),
                    int.MinValue,
                    int.MaxValue,
                    _ => new Command_ScreenMove(),
                    "�ʒu","X","Y",10)
                { Margin = new Thickness(0, 0, 0, 10) };



                ParameterSection p2 = new() { SectionTitle = "�`��" };
                p2.AddParameters(new() { zoom_p, pos_p });
                Parameters.Add(p2);
            }
            #endregion



            #region AudioObject
            var audioVM = objs.Select(o => o as IAudioScreenObjectVM).NonNull();

            if (audioVM.Any())
            {

                var volume_p = new Parameter_Double(
                    audioVM.ConvertAll(o => o.Volume),
                    0,
                    int.MaxValue,
                    _ => new Command_ChangeVolume(),
                    "����", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };

                var playback_p = new Parameter_Double(
                audioVM.ConvertAll(o => o.PlaybackRate),
                int.MinValue,
                int.MaxValue,
                _ => new Command_ChangePlaybackRate(),
                "�Đ����x", 0.1)
                            { Margin = new Thickness(0, 0, 0, 10) };

                var fade_p = new Parameter_DoubleDouble(
                    audioVM.ConvertAll(o => o.FadeIn),
                    audioVM.ConvertAll(o => o.FadeOut),
                    0,
                    int.MaxValue,
                    _ => new Command_ChangeFade(),
                    "�t�F�[�h", "IN", "OUT", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };

                var echoInterval_p = new Parameter_Double(
                audioVM.ConvertAll(o => o.EchoInterval),
                0,
                int.MaxValue,
                _ => new Command_ChangePlaybackRate(),
                "�G�R�[���x", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };
                

                ParameterSection p2 = new() { SectionTitle = "�����@���y" };
                p2.AddParameters(new() { volume_p, playback_p, fade_p, echoInterval_p });
                Parameters.Add(p2);
            }
            #endregion



            #region TextObject
            var textVM = objs.Select(o => o as ITextScreenObjVM).NonNull();

            if (textVM.Any())
            {

                var text_p = new Parameter_Text(
                    textVM.ConvertAll(o => o.Text),
                    _ => new Command_ChangeText(),

                    "�e�L�X�g")
                { Margin = new Thickness(0, 0, 0, 10) };

                var fontsize_p = new Parameter_Double(
                textVM.ConvertAll(o => o.FontSize),
                0,
                int.MaxValue,
                _ => new Command_ChangeFontSize(),
                "�t�H���g�T�C�Y", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };

                var fontColor_p = new Parameter_Color(
                textVM.ConvertAll(o => o.Color),
                _ => new Command_ChangeColor(),
                "�t�H���g�F")
                { Margin = new Thickness(0, 0, 0, 10) };

                ParameterSection p2 = new() { SectionTitle = "�e�L�X�g" };
                p2.AddParameters(new() { text_p, fontsize_p, fontColor_p });
                Parameters.Add(p2);
            }
            #endregion



        }









        //private void OnChangeSelectedItem(List<ITimelineItemObjectViewModel> objs)
        //{
        //    Parameters.Clear();
        //    SelectedItems = new(objs);

        //    if (objs.Count==1)
        //    {

        //        var layer_p = new Parameter_Int() { Title = "���C���[", OnlyInt = true };
        //        layer_p.Initialize(new() { objs[0].Layer }, objs[0].ItemData.Layer, 0);
        //        layer_p.ViewModelParams = new() { objs[0].Layer };
        //        layer_p.ModelParams = new() { layer => new Command_Move(()=>new() { objs[0] }) };

        //        var frame_p = new Parameter_Int() { Title = "�J�n�t���[��", OnlyInt = true };
        //        frame_p.Initialize(new() { objs[0].Frame }, objs[0].ItemData.Frame, 0);
        //        //layer_p.OnDecided.Subscribe(d => {
        //        //    foreach (var item in objs)
        //        //    {
        //        //        item.ItemData.ChangeData((int)Math.Round(d), item.ItemData.Length, item.ItemData.Layer, item.ItemData.Offset);
        //        //    }
        //        //});

        //        var length_p = new Parameter_Int { Title = "�t���[������", OnlyInt = true }; 
        //        length_p.Initialize(new() { objs[0].Length }, objs[0].ItemData.Length, 0);
        //        //layer_p.OnDecided.Subscribe(d => {
        //        //    foreach (var item in objs)
        //        //    {
        //        //        item.ItemData.ChangeData(item.ItemData.Frame, (int)Math.Round(d), item.ItemData.Layer, item.ItemData.Offset);
        //        //    }
        //        //});

        //        ParameterSection p = new() { SectionTitle = "�^�C�����C��" };
        //        p.AddParameters(new() { layer_p, frame_p, length_p });
        //        Parameters.Add(p);
        //    }


        //    //if (objs.All(o=>o.ItemData is IRenderingItem))
        //    //{
        //    //    var pos_p = new Parameter_IntInt() { MainTitle = "�\���ʒu", SubTitle1="X", SubTitle2="Y",OnlyInt=false };
        //    //    var opacity_p = new Parameter_Int() { Title = "�s�����x", OnlyInt = false };
        //    //    var size_p = new Parameter_Int() { Title = "�g�嗦", OnlyInt = false };
        //    //    var rotation_p = new Parameter_Int() { Title = "��]", OnlyInt = false };
        //    //    var fade_p = new Parameter_IntInt() { MainTitle = "�t�F�[�h", SubTitle1 = "IN", SubTitle2 = "OUT", OnlyInt = false };


        //    //    ParameterSection p = new() { SectionTitle = "�`��" };
        //    //    p.AddParameters(new() { pos_p, opacity_p, size_p, rotation_p, fade_p });
        //    //    Parameters.Add(p);
        //    //}
        //}

        //private IReadOnlyPointer Pointer { get; set; } 
    }
}