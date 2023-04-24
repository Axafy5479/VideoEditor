using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Timeline.CustomControl;

namespace Timeline
{
    public class TimelineLayoutViewModel
    {


        public int LayerHeight { get; set; } = 30;
        public double CursorY1 => 0;

        private ReactiveProperty<double>? cursorY2 = null;
        public IReadOnlyReactiveProperty<double> CursorY2 => cursorY2 ??= new(LayerLabels.Count * LayerHeight);


        public ReactiveProperty<double> CursorX { get; } = new ReactiveProperty<double>(0d);

        public ReactiveProperty<double> LayerWidth { get; } = new ReactiveProperty<double>(1000d);


        public IReadOnlyReactiveProperty<int> MaxFrame => TimelineDataController.Instance.Length;

        public IReadOnlyReactiveProperty<double> PixelPerFrame => TimelineDataController.Instance.PixelPerFrame;


        public ReadOnlyReactiveCollection<string> LayerLabels => _LayerLabels.ToReadOnlyReactiveCollection();
        private ReactiveCollection<string> _LayerLabels { get; } = new()
        {
            "Layer 00", "Layer 01","Layer 02","Layer 03","Layer 04", "Layer 05", "Layer 06","Layer 07","Layer 08","Layer 09",
            "Layer 10", "Layer 11","Layer 12","Layer 13","Layer 14", "Layer 15", "Layer 16","Layer 17","Layer 18","Layer 19",
            "Layer 20", "Layer 21","Layer 22","Layer 23","Layer 24", "Layer 25", "Layer 26","Layer 27","Layer 28","Layer 29",
            "Layer 30", "Layer 31","Layer 32","Layer 33","Layer 34", "Layer 35", "Layer 36","Layer 37","Layer 38","Layer 39",
            "Layer 40", "Layer 41","Layer 42","Layer 43","Layer 44", "Layer 45", "Layer 46","Layer 47","Layer 48","Layer 49",
            "Layer 50", "Layer 51","Layer 52","Layer 53","Layer 54", "Layer 55", "Layer 56","Layer 57","Layer 58","Layer 59",
            "Layer 60", "Layer 61","Layer 62","Layer 63","Layer 64", "Layer 65", "Layer 66","Layer 67","Layer 68","Layer 69",
            "Layer 70", "Layer 71","Layer 72","Layer 73","Layer 74", "Layer 75", "Layer 76","Layer 77","Layer 78","Layer 79",
            "Layer 80", "Layer 81","Layer 82","Layer 83","Layer 84", "Layer 85", "Layer 86","Layer 87","Layer 88","Layer 89",
            "Layer 90", "Layer 91","Layer 92","Layer 93","Layer 94", "Layer 95", "Layer 96","Layer 97","Layer 98","Layer 99"
        };

        public Point RightClickScreenPoint { get; internal set; }

        public void AddLayer()
        {
            _LayerLabels.Add($"Layer {LayerLabels.Count}");
            cursorY2.Value = LayerHeight * LayerLabels.Count;
        }

        public void RemoveLastLayer()
        {
            _LayerLabels.RemoveAt(_LayerLabels.Count - 1);
            cursorY2.Value = LayerHeight * LayerLabels.Count;
        }

    }





}
