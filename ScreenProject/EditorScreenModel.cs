using ControlzEx.Standard;
using Data;
using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class EditorScreenModel
    {
        public EditorScreenModel()
        {
            ShowingItems = new();
            TimelineDataController.Instance.ShowingItems.Subscribe(OnShowingItemChanged);
            TimelineDataController.Instance.Playing.Subscribe(t=>Task.Run(()=>Play_Stop(t)));
        }

        public ReactiveCollection<IScreenObject> ScreenObjects { get; } = new();

        private Dictionary<ITimelineObjectViewModel, IScreenObject?> ShowingItems { get; }


        public void AddScreenObject(IScreenObject screenObj)
        {
            ScreenObjects.Add(screenObj);
            UpdateScreenObject();
        }

        //public bool RemoveScreenObject(IScreenObject screenObj)
        //{
        //    return 
        //}

        public void UpdateScreenObject()
        {
            var temp = ScreenObjects.ToList();
            temp.Sort((a, b) => a.TlItemObjVM.Layer.Value - b.TlItemObjVM.Layer.Value);

            ScreenObjects.Clear();
            foreach (var item in temp)
            {
                ScreenObjects.Add(item);
            }

        }

        private void Play_Stop(bool playing)
        {
            lock (ScreenObjects)
            {
                int frame = TimelineDataController.Instance.CurrentFrame.Value;
                foreach (var item in ScreenObjects)
                {

                    item.Show(frame, playing);
                    if (!playing)
                    {
                        item.Stop();
                    }
                }

                UpdateScreenObject();
            }
        }


        private int waiting = -1;
        private bool IsBusy = false;

        private void OnShowingItemChanged((int frame, HashSet<ITimelineObjectViewModel>? showingItems, bool isPlaying) showingItemsInfo)
        {
            waiting = showingItemsInfo.frame;
            if (IsBusy) return;
            IsBusy = true;

            HashSet<ITimelineObjectViewModel>? showingItems = showingItemsInfo.showingItems;
            int frame = showingItemsInfo.frame;
            if (showingItems is null)
            {
                showingItems = new();
                IsBusy = false;
                return;
            }

            HashSet<ITimelineObjectViewModel> stayItems = new();
            foreach (var item in ShowingItems.Keys)
            {
                if(showingItems.Any(d=>d.Frame == item.Frame && d.Length == item.Length && d.Layer == item.Layer))
                {
                    stayItems.Add(item);
                }
            }

            HashSet<ITimelineObjectViewModel> newItems = new();
            foreach (var item in showingItems)
            {
                if (ShowingItems.Keys.All(d => d.Frame != item.Frame || d.Length != item.Length || d.Layer != item.Layer))
                {
                    newItems.Add(item);
                }
            }

            HashSet<ITimelineObjectViewModel> removingItems = new();
            foreach (var item in ShowingItems.Keys)
            {
                if (showingItems.All(d => d.Frame != item.Frame || d.Length != item.Length || d.Layer != item.Layer))
                {
                    removingItems.Add(item);
                }
            }

            
            List<IScreenObject> removingScreenObjs = new();
            foreach (var item in removingItems)
            {
                ShowingItems.Remove(item, out var screenObj);
                //removingScreenObjs.Add(screenObj);
                ScreenObjects.Remove(screenObj);

                screenObj.Dispose();
            }
            UpdateScreenObject();



            if (!newItems.Any())
            {
                IsBusy = false;
                return;
            }

            var tasks = new List<Task>();
            queue.Clear();

            foreach (var item in newItems)
            {
                tasks.Add(MakeScreenObjAsync(item));
            }

            Task.Run(() => {
                Task.WaitAll(tasks.ToArray());
            }).ContinueWith((t) =>
            {

                if (queue.Any())
                {
                    int frame = TimelineDataController.Instance.CurrentFrame.Value;

                    foreach (var item in ScreenObjects)
                    {
                        item.Show(frame, showingItemsInfo.isPlaying);
                        if (!showingItemsInfo.isPlaying)
                        {
                            item.Stop();
                        }
                    }

                    foreach (var item in queue)
                    {
                        item.Show(frame, showingItemsInfo.isPlaying);
                        if (!showingItemsInfo.isPlaying)
                        {
                            item.Stop();
                        }
                    }

                    foreach (var item in queue)
                    {
                        ScreenObjects.Add(item);
                    }
                    //ScreenObjects.AddRangeOnScheduler(queue);

                    UpdateScreenObject();
                }

                IsBusy = false;

            });

   

        }

        private ConcurrentQueue<IScreenObject> queue = new();

        private async Task MakeScreenObjAsync(ITimelineObjectViewModel item)
        {
            ShowingItems.Add(item, null);

            IScreenObject screenObj = item switch
            {
                VideoObjViewModel videoItem => new VideoScreenObject(videoItem.FilePath, videoItem),
                AudioObjViewModel audioItem => new AudioScreenObject(audioItem, audioItem.FilePath),
                ImageObjViewModel imageItem => new ImageScreenObject(imageItem.FilePath, imageItem),
                TextObjViewModel textItem => new TextScreenObject(textItem),
                VoiceObjViewModel voiceItem => new VoiceScreenObject(voiceItem),
                _ => throw new NotImplementedException()
            };

            queue.Enqueue(screenObj);

            if (screenObj != null)
            {
                ShowingItems[item] = screenObj;
            }
        }
    }
}
