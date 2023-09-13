using UnityEngine;

namespace SignVisionAI.Data
{
    public class RotatedBounds
    {
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;
        public Vector3 min
        {
            get
            {
                Vector3 halfSize = size * 0.5f;
                return center - rotation * halfSize;
            }
        }

        public Vector3 max
        {
            get
            {
                Vector3 halfSize = size * 0.5f;
                return center + rotation * halfSize;
            }
        }

        public RotatedBounds(Vector3 center, Vector3 size, Quaternion rotation)
        {
            this.center = center;
            this.size = size;
            this.rotation = rotation;
        }
        
        public bool Contains(Vector3 point)
        {
            // Inverse-rotate the point based on the bounds' rotation
            Vector3 localPoint = Quaternion.Inverse(rotation) * (point - center);
    
            // Check if the point is within the half extents
            return Mathf.Abs(localPoint.x) <= size.x * 0.5f &&
                   Mathf.Abs(localPoint.y) <= size.y * 0.5f &&
                   Mathf.Abs(localPoint.z) <= size.z * 0.5f;
        }

        public Vector3 NormalizedPositionInBounds(Vector3 otherPosition)
        {
            var relativePosition = otherPosition - this.min;
            var horizontalDistance = Vector3.Dot(relativePosition, rotation * Vector3.right);
            var verticalDistance = Vector3.Dot(relativePosition, rotation * Vector3.up);
            var depthDistance = Vector3.Dot(relativePosition, rotation * Vector3.forward);
            return new Vector3(
                Mathf.Clamp01(horizontalDistance / this.size.x),
                Mathf.Clamp01(verticalDistance / this.size.y),
                Mathf.Clamp01(depthDistance / this.size.z)
            );
        }
    }
}