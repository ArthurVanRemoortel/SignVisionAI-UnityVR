using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SignVisionAI.Data;

namespace SignVisionAI
{
    // [JsonObject]
    public class GestureRecording
    {
        private List<GestureFrame> Frames = new List<GestureFrame>();
        
        public void AddFrame(GestureFrame frame)
        {
            Frames.Add(frame);
        }        
        public int Length()
        {
            return Frames.Count;
        }

        public string ToJson(JsonSerializerSettings jsonSettings)
        {
            return JsonConvert.SerializeObject(this.Frames, Formatting.Indented, jsonSettings);
        }
        public void Clear()
        {
            Frames.Clear();
        }

        public Dictionary<int, GestureFrame> FramesAsDictionary()
        {
            Dictionary<int, GestureFrame> framesAsDictionary = new Dictionary<int, GestureFrame>();
            for (int i = 0; i < Frames.Count; i++)
            {
                framesAsDictionary.Add(i, Frames[i]);
            }
            return framesAsDictionary;
        }
    }
}