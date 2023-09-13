using UnityEngine;
using Newtonsoft.Json;

namespace SignVisionAI.Data
{
    [JsonObject]
    public struct GestureFrame
    {
        [JsonProperty(Order = 0)] public HandFrame LEFT;
        [JsonProperty(Order = 1)] public HandFrame RIGHT;
        [JsonProperty(Order = 2)] public Vector3 MOUTH;
        
        public GestureFrame(HandFrame left, HandFrame right, Vector3 mouth)
        {
            LEFT = left;
            RIGHT = right;
            MOUTH = mouth;
        }
    }
}