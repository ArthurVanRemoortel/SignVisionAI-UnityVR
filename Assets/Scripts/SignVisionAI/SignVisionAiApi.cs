using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine.Networking;
using SignVisionAI.Data;

namespace SignVisionAI
{
    [System.Serializable]
    public class ClassificationResult
    {
        public Dictionary<string, float> Predictions;
        
        public (string, float) BestPrediction()
        {
            float bestScore = 0;
            string bestPrediction = "";
            foreach (KeyValuePair<string, float> prediction in Predictions)
            {
                if (prediction.Value > bestScore)
                {
                    bestScore = prediction.Value;
                    bestPrediction = prediction.Key;
                }
            }
            return (bestPrediction, bestScore);
        }
    }
    
    public class SignVisionAiApi: MonoBehaviour
    {
        [SerializeField] private string APIUrl = "http://signvision.arthurvanremoortel.me";

        public static SignVisionAiApi Singleton { get; private set; }
        private JsonSerializerSettings jsonSettings;
        
        private void Awake() 
        {
            if (Singleton != null && Singleton != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Singleton = this; 
            } 
        }

        private void Start()
        {
            jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignore self-referencing loops
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy(), // Optional: use camelCase property names
                },
                Converters = new List<JsonConverter>
                {
                    new Vector3Converter(), // Add Unity's Vector3 converter
                },
            };
            var gestureFrame = new GestureFrame(new HandFrame(), new HandFrame(), new Vector3());
            string json = JsonConvert.SerializeObject(gestureFrame, Formatting.Indented, jsonSettings);
        }


        public IEnumerator ClassifyGesture(GestureRecording gestureRecording, string language)
        {
            Debug.Log("Classifying...");
            Dictionary<string, object> jsonObject = new Dictionary<string, object>();
            jsonObject["language"] = language;
            jsonObject["gesture"] = gestureRecording.FramesAsDictionary();
            string jsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, jsonSettings);
            UnityWebRequest request = UnityWebRequest.PostWwwForm(APIUrl + "/api/classify_gesture/", jsonData);
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("POST request failed: " + request.error + " - " + request.downloadHandler.text);
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response from API: " + responseText);
                ClassificationResult result = JsonConvert.DeserializeObject<ClassificationResult>(responseText);
                SignVisionAI.Singleton.OnGestureClassified.Invoke(result);
                
            }
        }
        
    }
}