using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using NAudioProj;
using OpenCVProj;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;
using Timeline.Error;

namespace Data
{
    [Union(0, typeof(AudioItem))]
    [Union(1, typeof(VideoItem))]
    [Union(2, typeof(ImageItem))]
    [Union(3, typeof(TextItem))]
    [Union(4, typeof(VoiceItem))]
    [MessagePackObject(keyAsPropertyName: true)]
    public abstract partial class TimelineItem : ObservableObject
    {
        public static (TimelineItem?, ErrorInfo?) CrerateItemFromFile(string filePath, int layer, int frame)
        {
            var extention = Path.GetExtension(filePath).ToLower();
            return extention switch
            {
                ".mp3" => (new AudioItem(filePath, layer, frame, AudioLength(filePath)), null),
                ".wav" => (new AudioItem(filePath, layer, frame, AudioLength(filePath)), null),

                ".mkv" => (new VideoItem(filePath, layer, frame, VideoLength(filePath)), null),
                ".avi" => (new VideoItem(filePath, layer, frame, VideoLength(filePath)), null),
                ".mp4" => (new VideoItem(filePath, layer, frame, VideoLength(filePath)), null),
                ".mov" => (new VideoItem(filePath, layer, frame, VideoLength(filePath)), null),

                ".png" => (new ImageItem(filePath, layer, frame, 300), null),
                ".jpg" => (new ImageItem(filePath, layer, frame, 300), null),
                ".bmp" => (new ImageItem(filePath, layer, frame, 300), null),

                ".txt" => (new TextItem(layer, frame, 300), null),

                _ => (null, new(ErrorCode.InputFileExtentionCantBeUsed))
            };
        }



        private static int VideoLength(string filePath)
        {
            var videoInfo = VideoUtil.GetInfo(filePath);
            return (int)Math.Round(videoInfo.duration * 60);
        }

        private static int AudioLength(string filePath)
        {
            var audioInfo = AudioUtil.GetAudioInfo(filePath);
            return (int)Math.Round(audioInfo.lengthSec * 60);
        }

        public TimelineItem(string itemName, int layer, int l, int _length)
        {
            ItemName = itemName;
            Frame = l;
            Layer = layer;
            Length = _length;
        }

        [SerializationConstructor]
        public TimelineItem() { }

        #region 実際にデータとして保存される量
        [ObservableProperty] private string _ItemName = "DefaultName";
        [ObservableProperty] private int _Frame = 0;
        [ObservableProperty] private int _Length = 1;
        [ObservableProperty] private int _Layer = 0;
        #endregion

        [IgnoreMember]
        public int R => Frame + Length;

        [IgnoreMember]
        public abstract FileType FileType { get; }

        [IgnoreMember]
        private int offset;
        public int Offset { get => offset; set => this.SetProperty(ref this.offset, value); }


        [IgnoreMember]
        public bool Cloned { get; set; } = false;


        public (TimelineItem?, ErrorInfo?) CreateClone(int layer, int l)
        {
            (byte[]? serializedData, var e1) = Serialize();
            if (serializedData == null)
            {
                return (null, e1);
            }


            (var clone, var e2) = Deserialize(serializedData);

            if (clone != null && e2 == null)
            {
                clone.Cloned = true;
                clone.Layer = layer;
                clone.Frame = l;
                return (clone, null);
            }
            else
            {
                return (null, e2);
            }
        }

        public (byte[]?, ErrorInfo?) Serialize()
        {
            try
            {
                return (MessagePackSerializer.Serialize(this), null);
            }
            catch (Exception e)
            {
                return (null, new(ErrorCode.SerializingFailed, e));
            }
        }

        public static (TimelineItem?, ErrorInfo?) Deserialize(byte[] byteData)
        {
            try
            {
                var newTLItem = MessagePackSerializer.Deserialize<TimelineItem>(byteData);
                return (newTLItem, null);
            }
            catch (Exception e)
            {
                return (null, new(ErrorCode.DeserializingFailed, e));
            }
        }
    }
}
