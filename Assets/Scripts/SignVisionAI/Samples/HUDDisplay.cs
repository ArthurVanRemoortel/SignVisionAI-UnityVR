using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SignVisionAI.Samples
{
    public class HUDDisplay: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Subtitle;

        private void Start()
        {
            SignVisionAI.Singleton.OnGestureClassified.AddListener(OnGestureClassified);
        }
        
        private void OnGestureClassified(ClassificationResult result)
        {
            var (label, score) = result.BestPrediction();
            Subtitle.text =  $"{label} ({score})";
        }
    }
}