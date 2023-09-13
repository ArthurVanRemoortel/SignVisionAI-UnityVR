using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using UnityEngine.Networking;
using SignVisionAI.Data;
using UnityEngine.Events;

namespace SignVisionAI
{
    [System.Serializable]
    public class Hand
    {
        public GameObject Wrist;
        public GameObject ThumbBase;
        public GameObject ThumbTip;
        public GameObject IndexBase;
        public GameObject IndexTip;
        public GameObject MiddleBase;
        public GameObject MiddleTip;
        public GameObject RingBase;
        public GameObject RingTip;
        public GameObject PinkyBase;
        public GameObject PinkyTip;

        public FieldInfo[] GetFields()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            return fields;
        }
    }

    public class SignVisionAI : MonoBehaviour
    {
        [Header("General")] 
        public string SignLanguage = "VGT";
        
        [Header("Recorder Bounds")] 
        [SerializeField] private Camera VRCamera;
        [SerializeField][Range(-1, 1)] private float YOffset = 0.0f;
        [SerializeField][Range(0, 2)] private float BoxWidth = 1.0f;
        [SerializeField][Range(0, 2)] private float BoxHeight = 1.0f;
        [SerializeField][Range(0, 2)] private float BoxDepth = 1.0f;
        [SerializeField][Range(10, 60)] private int SamplesPerSeconds = 30; // 30 times per second

        [Header("Body")]
        [SerializeField] private Transform MouthTransform;
        [SerializeField] private Hand LeftHand;
        [SerializeField] private Hand RightHand;
        
        [System.Serializable]
        public class RecordingStartedEvent : UnityEvent<GestureRecording> { }        
        public RecordingStartedEvent OnGestureRecordingStarted;
                
        [System.Serializable]
        public class RecordingStoppedEvent : UnityEvent<GestureRecording> { }        
        public RecordingStoppedEvent OnGestureRecordingStopped;
        
        [System.Serializable]
        public class ClassifiedEvent : UnityEvent<ClassificationResult> { }
        public ClassifiedEvent OnGestureClassified;
        
        public static SignVisionAI Singleton { get; private set; }
        
        public bool Started { get; private set; }
        public bool IsRecording { get; private set; }
        
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
        void Start()
        {
            Invoke(nameof(DelayedStart), 3f);
        }
        
        void DelayedStart()
        {
            Started = true;
            StartCoroutine(SamplePosition());
        }
        
        public RotatedBounds GetRotatedDetectionBounds()
        {
            var cameraTransform = VRCamera.transform;
            var cameraPosition = cameraTransform.position;
            var center = new Vector3(cameraPosition.x, cameraPosition.y + YOffset, cameraPosition.z);
            return new RotatedBounds(center, new Vector3(BoxWidth, BoxHeight, BoxDepth), cameraTransform.rotation);
        }
        
        private IEnumerator SamplePosition()
        {
            var gestureRecording = new GestureRecording();
            while (Started)
            {
                var rotatedDetectionBounds = GetRotatedDetectionBounds();
                // var cameraPosition = VRCamera.transform.position;
                var leftInBounds = rotatedDetectionBounds.Contains(LeftHand.Wrist.transform.position);
                var rightInBounds = rotatedDetectionBounds.Contains(RightHand.Wrist.transform.position);
                var mouthPosition = rotatedDetectionBounds.NormalizedPositionInBounds(MouthTransform.position);
                if (leftInBounds || rightInBounds)
                {
                    if (!IsRecording)
                    {
                        OnGestureRecordingStarted.Invoke(gestureRecording);
                    }
                    IsRecording = true;
                    HandFrame leftFrame = HandFrame.Empty();
                    HandFrame rightFrame = HandFrame.Empty();
                    if (leftInBounds)
                    {
                        leftFrame = new HandFrame(LeftHand, rotatedDetectionBounds);
                    }
                    if (rightInBounds)
                    {
                        rightFrame = new HandFrame(RightHand, rotatedDetectionBounds);
                    }
                    var gestureFrame = new GestureFrame(leftFrame, rightFrame, mouthPosition);
                    // string json = JsonConvert.SerializeObject(gestureFrame, Formatting.Indented, jsonSettings);
                    gestureRecording.AddFrame(gestureFrame);
                }
                else
                {
                    if (IsRecording)
                    {
                        OnGestureRecordingStopped.Invoke(gestureRecording);
                    }
                    IsRecording = false;
                    if (gestureRecording.Length() > 10)
                    {
                        StartCoroutine(SignVisionAiApi.Singleton.ClassifyGesture(gestureRecording, SignLanguage));
                    }
                    gestureRecording.Clear();
                }
                yield return new WaitForSeconds(1f / SamplesPerSeconds);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = IsRecording ? Color.green : Color.yellow;
            Gizmos.DrawSphere(MouthTransform.position, 0.03f);
            
            Gizmos.color = IsRecording ? Color.green : Color.yellow;
            RotatedBounds rotatedDetectionBounds = GetRotatedDetectionBounds();
            float halfWidth = rotatedDetectionBounds.size.x / 2f;
            float halfHeight = rotatedDetectionBounds.size.y / 2f;

            Vector3 topLeft = rotatedDetectionBounds.center + rotatedDetectionBounds.rotation * new Vector3(-halfWidth, halfHeight, 0f);
            Vector3 topRight = rotatedDetectionBounds.center + rotatedDetectionBounds.rotation * new Vector3(halfWidth, halfHeight, 0f);
            Vector3 bottomLeft = rotatedDetectionBounds.center + rotatedDetectionBounds.rotation * new Vector3(-halfWidth, -halfHeight, 0f);
            Vector3 bottomRight = rotatedDetectionBounds.center + rotatedDetectionBounds.rotation * new Vector3(halfWidth, -halfHeight, 0f);

            // Draw the rectangle using Gizmos.DrawLine
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
        
    }
}
