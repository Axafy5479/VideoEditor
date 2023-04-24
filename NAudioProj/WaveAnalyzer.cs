using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioProj
{
    internal class WaveHeader
    {
        public string RiffHeader { get; internal set; }
        public int FileSize { get; internal set; }
        public string Header { get; internal set; }
        public string FormatChunk { get; internal set; }
        public int FormatChunkSize { get; internal set; }
        public short FormatID { get; internal set; }
        public short Channel { get; internal set; }
        public int SampleRate { get; internal set; }
        public int BytePerSec { get; internal set; }
        public short BlockSize { get; internal set; }
        public short BitPerSample { get; internal set; }
        public string DataChunk { get; internal set; }
        public int DataChunkSize { get; internal set; }
        public int PlayTimeMsec { get; internal set; }
    }


    internal static class WaveAnalyzer
    {
        public static bool ChangeSamplingRate(byte[] waveByte, int samplingRate,double pitch)
        {
            //Debug.WriteLine("ReadWave : " + waveFilePath);


            using (MemoryStream fs = new MemoryStream(waveByte))
            {
                try
                {
                    BinaryWriter br = new BinaryWriter(fs);
                    //4bytes 固定文字列 "RIFF"
                    //waveHeader.RiffHeader = Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));

                    //4bytes ファイルサイズ
                    //waveHeader.FileSize = BitConverter.ToInt32(br.ReadBytes(4), 0);

                    //4bytes 固定文字列 "WAVE"
                    //waveHeader.Header = Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));

                    //4bytes ChunkIDを取得する
                    //string chunk = Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));

                    // 4bytes ChunkSize
                    //waveHeader.FormatChunkSize = BitConverter.ToInt32(br.ReadBytes(4), 0);

                    //2bytes FormatID
                    //waveHeader.FormatID = BitConverter.ToInt16(br.ReadBytes(2), 0);

                    //2bytes チャンネル
                    //waveHeader.Channel = BitConverter.ToInt16(br.ReadBytes(2), 0);

                    // 4bytes サンプリングレート
                    fs.Position = 24;
                    fs.Write(BitConverter.GetBytes((int)(samplingRate*pitch)), 0, 4);
                }
                catch
                {
                    fs.Close();
                    return false;
                }
            }

            return true;
        }

        public static void ReadWave(byte[] waveByte)
        {
            WaveHeader waveHeader = new WaveHeader();

            using (MemoryStream fs = new MemoryStream(waveByte))
            {
                try
                {
                    BinaryReader br = new BinaryReader(fs);
                    waveHeader.RiffHeader = Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));
                    waveHeader.FileSize = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    waveHeader.Header = Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));

                    bool readFmtChunk = false;
                    bool readDataChunk = false;
                    while (!readFmtChunk || !readDataChunk)
                    {
                        // ChunkIDを取得する
                        string chunk = Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));

                        if (chunk.ToLower().CompareTo("fmt ") == 0)
                        {
                            //    Debug.WriteLine("fmt : " + waveFilePath);
                            // fmtチャンクの読み込み
                            waveHeader.FormatChunk = chunk;
                            waveHeader.FormatChunkSize = BitConverter.ToInt32(br.ReadBytes(4), 0);
                            waveHeader.FormatID = BitConverter.ToInt16(br.ReadBytes(2), 0);
                            waveHeader.Channel = BitConverter.ToInt16(br.ReadBytes(2), 0);
                            waveHeader.SampleRate = BitConverter.ToInt32(br.ReadBytes(4), 0);
                            waveHeader.BytePerSec = BitConverter.ToInt32(br.ReadBytes(4), 0);
                            waveHeader.BlockSize = BitConverter.ToInt16(br.ReadBytes(2), 0);
                            waveHeader.BitPerSample = BitConverter.ToInt16(br.ReadBytes(2), 0);

                            readFmtChunk = true;
                        }
                        else if (chunk.ToLower().CompareTo("data") == 0)
                        {
                            //   Debug.WriteLine("data : ");
                            // dataチャンクの読み込み
                            waveHeader.DataChunk = chunk;
                            waveHeader.DataChunkSize = BitConverter.ToInt32(br.ReadBytes(4), 0);

                            var waveData = br.ReadBytes(waveHeader.DataChunkSize);
                            //  Debug.WriteLine(string.Format("waveData : {0:X} {1:X}", waveData[0], waveData[1]));

                            // 再生時間を算出する
                            int bytesPerSec = waveHeader.SampleRate * waveHeader.BlockSize;
                            waveHeader.PlayTimeMsec = (int)(((double)waveHeader.DataChunkSize / (double)bytesPerSec) * 1000);

                            readDataChunk = true;
                        }
                        else
                        {
                            // 不要なチャンクの読み捨て
                            int size = BitConverter.ToInt32(br.ReadBytes(4), 0);
                            if (0 < size)
                            {
                                br.ReadBytes(size);
                            }
                        }
                    }
                }
                catch
                {
                    fs.Close();
                    //return false;
                }
            }
        }
    }
}
