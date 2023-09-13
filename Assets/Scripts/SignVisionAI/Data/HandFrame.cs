using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace SignVisionAI.Data
{
    [JsonObject]
    public class Coordinate
    {
        [JsonProperty(Order = 0)] public float? x;
        [JsonProperty(Order = 1)] public float? y;
        [JsonProperty(Order = 2)] public float? z;
        
        public Coordinate(float? x, float? y, float? z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public Coordinate(Vector3 vector)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }

        public static Coordinate Empty()
        {
            return new Coordinate(null, null, null);
        }
    }
    
    [JsonObject]
    public struct HandFrame
    {
        [JsonProperty(Order = 0)] public Coordinate WRIST;
        [JsonProperty(Order = 1)] public Coordinate THUMB_BASE;
        [JsonProperty(Order = 2)] public Coordinate THUMB_TIP;
        [JsonProperty(Order = 3)] public Coordinate INDEX_BASE;
        [JsonProperty(Order = 4)] public Coordinate INDEX_TIP;
        [JsonProperty(Order = 5)] public Coordinate MIDDLE_BASE;
        [JsonProperty(Order = 6)] public Coordinate MIDDLE_TIP;
        [JsonProperty(Order = 7)] public Coordinate RING_BASE;
        [JsonProperty(Order = 8)] public Coordinate RING_TIP;
        [JsonProperty(Order = 9)] public Coordinate PINKY_BASE;
        [JsonProperty(Order = 10)] public Coordinate PINKY_TIP;

        public static HandFrame Empty()
        {
            HandFrame frame = new HandFrame();
            frame.WRIST = Coordinate.Empty();
            frame.THUMB_TIP = Coordinate.Empty();
            frame.THUMB_BASE = Coordinate.Empty();
            frame.INDEX_TIP = Coordinate.Empty();
            frame.INDEX_BASE = Coordinate.Empty();
            frame.MIDDLE_TIP = Coordinate.Empty();
            frame.MIDDLE_BASE = Coordinate.Empty();
            frame.RING_TIP = Coordinate.Empty();
            frame.RING_BASE = Coordinate.Empty();
            frame.PINKY_TIP = Coordinate.Empty();
            frame.PINKY_BASE = Coordinate.Empty();
            return frame;
        }

        public HandFrame(Hand hand, RotatedBounds detectionBounds)
        {
            var values = new Dictionary<string, Coordinate>();
            foreach (FieldInfo field in hand.GetFields())
            {
                var fieldValue = (GameObject) field.GetValue(hand);
                var fieldName = Regex.Replace(field.Name, @"(\p{Lu})", "_$1").ToUpper().Substring(1);
                var fieldPosition = fieldValue.transform.position;
                if (fieldPosition == Vector3.zero)
                {
                    values.Add(fieldName, new Coordinate(null, null, null));
                }
                else
                {
                    var positionWithingBounds = detectionBounds.NormalizedPositionInBounds(fieldPosition);
                    values.Add(fieldName, new Coordinate(positionWithingBounds));   
                }
            }
            WRIST = values["WRIST"];
            THUMB_TIP = values["THUMB_TIP"];
            THUMB_BASE = values["THUMB_BASE"];
            INDEX_TIP = values["INDEX_TIP"];
            INDEX_BASE = values["INDEX_BASE"];
            MIDDLE_TIP = values["MIDDLE_TIP"];
            MIDDLE_BASE = values["MIDDLE_BASE"];
            RING_TIP = values["RING_TIP"];
            RING_BASE = values["RING_BASE"];
            PINKY_TIP = values["PINKY_TIP"];
            PINKY_BASE = values["PINKY_BASE"];
        }
    }
}