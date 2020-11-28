using UnityEngine;

namespace VileRik.Orthographic2DCameraController
{
    public class OrthographicCameraState
    {
        public Vector3 position;
        public Quaternion rotation = Quaternion.Euler(0, 0, 0);
        public float size;

        public override bool Equals(object obj)
        {
            return Equals(obj as OrthographicCameraState);
        }

        public bool Equals(OrthographicCameraState other)
        {
            return other is OrthographicCameraState && 
                position == other.position &&
                size == other.size &&
                rotation == other.rotation;
        }

        public override int GetHashCode() => (position, rotation, size).GetHashCode();
    }
}