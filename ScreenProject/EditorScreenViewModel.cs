using Reactive.Bindings;

namespace ScreenProject
{
    public class EditorScreenViewModel
    {
        public EditorScreenViewModel() 
        {
            Model = new EditorScreenModel();
            ScreenObjects = Model.ScreenObjects.ToReadOnlyReactiveCollection();
        }

        public ReadOnlyReactiveCollection<IScreenObject> ScreenObjects { get; }
        private EditorScreenModel Model { get; }

        public void AddScreenObject(IScreenObject screenObj)
        {
            Model.AddScreenObject(screenObj);
        }

        //public void RemoveScreenObject(IScreenObject screenObj)
        //{
        //    Model.RemoveScreenObject(screenObj);
        //}

        public void UpdateScreenObject()
        {
            Model.UpdateScreenObject();
        }

        public ReactiveProperty<double> PixelWidth { get; } = new(1920);
        public ReactiveProperty<double> PixelHeight { get; } = new(1080);

        public ReactiveProperty<double> ScaleX { get; } = new(1);
        public ReactiveProperty<double> ScaleY { get; } = new(1);

        public ReactiveProperty<double> CenterX { get; } = new(0);
        public ReactiveProperty<double> CenterY { get; } = new(0);
    }
}
