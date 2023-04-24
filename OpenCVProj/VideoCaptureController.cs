using OpenCvSharp;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace OpenCVProj
{
    public class VideoCaptureController : IDisposable
    {
        private WriteableBitmap? Wb { get; set; }
        private VideoCapture? Capture { get; set; }
        private Image ImageUI { get; }
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// ����̒���(�b)
        /// </summary>
        public double TimeLength { get; }
        public double Width { get; }
        public double Height { get; }

        public VideoCaptureController(string uri, Image imageUI)
        {
            Capture = new VideoCapture(uri);

            TimeLength = Capture.FrameCount / (double)Capture.Fps;
            Width = Capture.FrameWidth;
            Height = Capture.FrameHeight;

            if (Width == 0)
            {
                MessageBox.Show("VideoCapture�̃C���X�^���X�����Ɏ��s���܂���");
            }
            else
            {

                // �L���v�`�������摜�̃R�s�[��ƂȂ�WriteableBitmap���쐬
                Wb = new WriteableBitmap(Capture.FrameWidth, Capture.FrameHeight, 96, 96, PixelFormats.Rgb24, null);

                ImageUI = imageUI;
            }

        }

        public void Dispose()
        {
            _Dispose();
        }

        private async void _Dispose()
        {
            while (IsRunning)
            {
                await Task.Delay(100);
            }

            Wb = null;
            Capture?.Dispose();
            Capture = null;
        }

        public async Task CaptureAsync(double time)
        {

            if (Capture == null || Wb == null)
            {
                MessageBox.Show("�����ǂݍ��߂܂���ł���");
                return;
            }

            if (IsRunning) return;
            IsRunning = true;


            int frame = (int)(time * Capture.Fps);

            // �t���[���摜��񓯊��Ɏ擾
            var image = await QueryFrameAsync(frame, Capture);
            if (image == null) return;

            Wb = image.ToWriteableBitmap();
            ImageUI.Source = Wb;

            IsRunning = false;
        }

        private async Task<Mat> QueryFrameAsync(int frame,VideoCapture capture)
        {

            // await�ł���`�ŁA�񓯊��Ƀt���[���̎擾���s���܂��B
            return await Task.Run(() =>
            {
                capture.PosFrames = frame;
                Mat im=new Mat();
                capture.Read(im);
                return im;
            });
        }
    }
}
