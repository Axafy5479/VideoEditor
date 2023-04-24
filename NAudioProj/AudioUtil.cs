using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioProj
{
    public static class AudioUtil
    {
        public static (double bytePerSec, double lengthSec) GetAudioInfo(string url)
        {
            var audioStream = new AudioFileReader(url);

            var ByteParSec = (audioStream.WaveFormat.BitsPerSample / 8) * audioStream.WaveFormat.SampleRate * audioStream.WaveFormat.Channels;
            var AllLengthSec = ((double)audioStream.Length) / ByteParSec;
            audioStream.Dispose();

            return (ByteParSec, AllLengthSec);
        }
    }
}
