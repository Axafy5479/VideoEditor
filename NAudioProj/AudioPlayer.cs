using NAudio.Wave;
using System;
using System.Threading.Tasks;

namespace NAudioProj
{
    public class AudioPlayer
    {
        public AudioPlayer(string uri)
        {
            var audioStream = new AudioFileReader(uri);

            ByteParSec = (audioStream.WaveFormat.BitsPerSample / 8) * audioStream.WaveFormat.SampleRate * audioStream.WaveFormat.Channels;
            AllLengthSec = ((double)audioStream.Length) / ByteParSec;
            audioStream.Dispose();
            FilePath = uri;
        }



        public double AllLengthSec { get; }
        private double ByteParSec { get; }
        public string FilePath { get; }

        public void ChangeVolume(double ratio)
        {
            if(outputDevice == null) return;
            outputDevice.Volume = (float)(Math.Max(ratio,0.00001));
        }

        public void ChangePlaybackSpeed(double ratio)
        {
            if (outputDevice == null) return;
            throw new NotImplementedException();
        }

        WaveOutEvent? outputDevice;
        public async Task Play(double latencySec, double delaySec,double volume)
        {
            int delay = (int)(delaySec * 1000);
            outputDevice = new WaveOutEvent();

            var afr = new AudioFileReader(FilePath);
            afr.Position = (long)(ByteParSec* latencySec);

            outputDevice.Init(afr);
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            outputDevice.Volume = (float)volume;
            outputDevice.Play();
        }


        public void Dispose()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
        }

     
    }
}