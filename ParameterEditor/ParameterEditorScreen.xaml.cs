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
            Debug.WriteLine("直前に削除したアイテムのパラメーターも編集を使用とする");


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
                "レイヤー")
            { Margin = new Thickness(0, 0, 0, 10) };

            var frame_p = new Parameter_Int(
            objs.ConvertAll(o => o.Frame),
            0,
            int.MaxValue, _ => new Command_Move(),
            "開始フレーム")
            { Margin = new Thickness(0, 0, 0, 10) };


            var length_p = new Parameter_Int(
            objs.ConvertAll(o => o.Length),
            0,
            int.MaxValue, _ => new Command_Move(),
            "フレーム長")
            { Margin = new Thickness(0, 0, 0, 10) };

            ParameterSection p = new() { SectionTitle = "タイムライン" };
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
                    "拡大率", 0.1)
                { Margin=new Thickness(0,0,0,10) };

                var pos_p = new Parameter_DoubleDouble(
                    renderVM.ConvertAll(o => o.X),
                    renderVM.ConvertAll(o => o.Y),
                    int.MinValue,
                    int.MaxValue,
                    _ => new Command_ScreenMove(),
                    "位置","X","Y",10)
                { Margin = new Thickness(0, 0, 0, 10) };



                ParameterSection p2 = new() { SectionTitle = "描画" };
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
                    "音量", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };

                var playback_p = new Parameter_Double(
                audioVM.ConvertAll(o => o.PlaybackRate),
                int.MinValue,
                int.MaxValue,
                _ => new Command_ChangePlaybackRate(),
                "再生速度", 0.1)
                            { Margin = new Thickness(0, 0, 0, 10) };

                var fade_p = new Parameter_DoubleDouble(
                    audioVM.ConvertAll(o => o.FadeIn),
                    audioVM.ConvertAll(o => o.FadeOut),
                    0,
                    int.MaxValue,
                    _ => new Command_ChangeFade(),
                    "フェード", "IN", "OUT", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };

                var echoInterval_p = new Parameter_Double(
                audioVM.ConvertAll(o => o.EchoInterval),
                0,
                int.MaxValue,
                _ => new Command_ChangePlaybackRate(),
                "エコー強度", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };
                

                ParameterSection p2 = new() { SectionTitle = "音声　音楽" };
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

                    "テキスト")
                { Margin = new Thickness(0, 0, 0, 10) };

                var fontsize_p = new Parameter_Double(
                textVM.ConvertAll(o => o.FontSize),
                0,
                int.MaxValue,
                _ => new Command_ChangeFontSize(),
                "フォントサイズ", 0.1)
                { Margin = new Thickness(0, 0, 0, 10) };

                var fontColor_p = new Parameter_Color(
                textVM.ConvertAll(o => o.Color),
                _ => new Command_ChangeColor(),
                "フォント色")
                { Margin = new Thickness(0, 0, 0, 10) };

                ParameterSection p2 = new() { SectionTitle = "テキスト" };
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

        //        var layer_p = new Parameter_Int() { Title = "レイヤー", OnlyInt = true };
        //        layer_p.Initialize(new() { objs[0].Layer }, objs[0].ItemData.Layer, 0);
        //        layer_p.ViewModelParams = new() { objs[0].Layer };
        //        layer_p.ModelParams = new() { layer => new Command_Move(()=>new() { objs[0] }) };

        //        var frame_p = new Parameter_Int() { Title = "開始フレーム", OnlyInt = true };
        //        frame_p.Initialize(new() { objs[0].Frame }, objs[0].ItemData.Frame, 0);
        //        //layer_p.OnDecided.Subscribe(d => {
        //        //    foreach (var item in objs)
        //        //    {
        //        //        item.ItemData.ChangeData((int)Math.Round(d), item.ItemData.Length, item.ItemData.Layer, item.ItemData.Offset);
        //        //    }
        //        //});

        //        var length_p = new Parameter_Int { Title = "フレーム長さ", OnlyInt = true }; 
        //        length_p.Initialize(new() { objs[0].Length }, objs[0].ItemData.Length, 0);
        //        //layer_p.OnDecided.Subscribe(d => {
        //        //    foreach (var item in objs)
        //        //    {
        //        //        item.ItemData.ChangeData(item.ItemData.Frame, (int)Math.Round(d), item.ItemData.Layer, item.ItemData.Offset);
        //        //    }
        //        //});

        //        ParameterSection p = new() { SectionTitle = "タイムライン" };
        //        p.AddParameters(new() { layer_p, frame_p, length_p });
        //        Parameters.Add(p);
        //    }


        //    //if (objs.All(o=>o.ItemData is IRenderingItem))
        //    //{
        //    //    var pos_p = new Parameter_IntInt() { MainTitle = "表示位置", SubTitle1="X", SubTitle2="Y",OnlyInt=false };
        //    //    var opacity_p = new Parameter_Int() { Title = "不透明度", OnlyInt = false };
        //    //    var size_p = new Parameter_Int() { Title = "拡大率", OnlyInt = false };
        //    //    var rotation_p = new Parameter_Int() { Title = "回転", OnlyInt = false };
        //    //    var fade_p = new Parameter_IntInt() { MainTitle = "フェード", SubTitle1 = "IN", SubTitle2 = "OUT", OnlyInt = false };


        //    //    ParameterSection p = new() { SectionTitle = "描画" };
        //    //    p.AddParameters(new() { pos_p, opacity_p, size_p, rotation_p, fade_p });
        //    //    Parameters.Add(p);
        //    //}
        //}

        //private IReadOnlyPointer Pointer { get; set; } 
    }
}