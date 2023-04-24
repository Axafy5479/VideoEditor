using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenCVProj
{
    public static class VideoUtil
    {
        public static (double duration, (int x,int y) size) GetInfo(string path)
        {
            var Capture = new VideoCapture(path);

            return (Capture.FrameCount / (double)Capture.Fps, (Capture.FrameWidth, Capture.FrameHeight));
        }
    }
}
