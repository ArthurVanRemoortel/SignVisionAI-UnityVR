using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;

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
            var values = new List<Coordinate>();
            foreach (FieldInfo field in hand.GetFields())
            {
                GameObject fieldValue = (GameObject) field.GetValue(hand);
                Vector3 fieldPosition = fieldValue.transform.position;
                if (fieldPosition == Vector3.zero)
                {
                    values.Add(new Coordinate(null, null, null));
                }
                else
                {
                    var positionWithingBounds = detectionBounds.NormalizedPositionInBounds(fieldPosition);
                    positionWithingBounds.y = 1 - positionWithingBounds.y;
                    Debug.Log(positionWithingBounds.x + ", " + positionWithingBounds.y);
                    values.Add(new Coordinate(positionWithingBounds));   
                }
            }
            WRIST = values[0];
            THUMB_TIP = values[1];
            THUMB_BASE = values[2];
            INDEX_TIP = values[3];
            INDEX_BASE = values[4];
            MIDDLE_TIP = values[5];
            MIDDLE_BASE = values[6];
            RING_TIP = values[7];
            RING_BASE = values[8];
            PINKY_TIP = values[9];
            PINKY_BASE = values[10];
        }
    }
}