using UnityEngine;

namespace AlternateReality
{
    public class VehicleSuspension : MonoBehaviour
    {
        public float Compression { get; set; }
        public Vector3 ImpactPoint { get; set; }
        public Vector3 ImpactNormal { get; set; }
        public bool IsGrounded { get; set; }
    }
}