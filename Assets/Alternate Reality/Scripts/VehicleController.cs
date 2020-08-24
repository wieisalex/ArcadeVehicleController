using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlternateReality
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] private VehicleSuspension[] _vehicleSuspensions;

        private Rigidbody _rigidbody;
        private Vector3 _average;
        private Vector3 _offset;

        private float _velocityForward;
        private float _velocityRotation;
        private float _distance;
        private float _force;
        private float _stiffness;
        private float _grip;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _average = new Vector3();
            _offset = new Vector3(0f, 0.35f, 1f);

            _velocityForward = 20f;
            _velocityRotation = 10f;
            _distance = 1.5f;
            _force = 20000f;
            _stiffness = 0.5f;
            _grip = 5000f;
        }


        private void FixedUpdate()
        {
            Suspension();
            Movement();
            Steering();
            Grip();
        }


        private void Suspension()
        {
            List<Vector3> normals = new List<Vector3>();

            for (int i = 0; i < _vehicleSuspensions.Length; i++)
            {
                VehicleSuspension suspension = _vehicleSuspensions[i];
                Transform t = suspension.transform;

                if (Physics.Raycast(t.position, t.TransformDirection(Vector3.down), out RaycastHit hit, _distance, ~(1 << 8)))
                {
                    suspension.Compression = Mathf.Abs(hit.distance / _distance - 1);
                    suspension.ImpactPoint = hit.point;
                    suspension.ImpactNormal = hit.normal;
                    suspension.IsGrounded = true;

                    normals.Add(suspension.ImpactNormal);

                    _rigidbody.AddForceAtPosition(Vector3.up * (suspension.Compression * _stiffness) * _force, t.position);

                    Debug.DrawRay(t.position, Vector3.ProjectOnPlane(transform.forward, suspension.ImpactNormal));
                }
                else
                {
                    suspension.IsGrounded = false;
                }
            }

            _average = Tools.GetAverageVector3(normals);

            Debug.DrawRay(transform.position + transform.TransformDirection(_offset), Vector3.ProjectOnPlane(transform.forward, _average) * 5f, Color.blue);
        }


        private void Movement()
        {
            if (IsGrounded())
            {
                _rigidbody.AddForceAtPosition(Vector3.ProjectOnPlane(transform.forward, _average) * Mathf.Clamp(Input.GetAxis("Vertical"), -0.5f, 1f) * _velocityForward, _rigidbody.position + transform.TransformDirection(_offset), ForceMode.Acceleration);
            }
        }


        private void Steering()
        {
            _rigidbody.AddTorque(new Vector3(0f, Input.GetAxis("Horizontal") * _velocityRotation, 0f), ForceMode.Acceleration);
        }


        private void Grip()
        {
            Vector3 local = transform.InverseTransformDirection(_rigidbody.velocity);

            _rigidbody.AddRelativeForce(local.x * Vector3.left * _grip);
        }


        private bool IsGrounded()
        {
            int grounded = 0;

            foreach (VehicleSuspension suspension in _vehicleSuspensions)
                if (suspension.IsGrounded)
                    grounded++;

            return grounded != 0;
        }
        


        private void OnDrawGizmos()
        {
            foreach (VehicleSuspension suspension in _vehicleSuspensions)
            {
                Transform t = suspension.transform;

                Gizmos.DrawWireSphere(t.position, 0.1f);

                Handles.Label(t.position + (t.TransformDirection(Vector3.down) * 0.25f), suspension.Compression.ToString("N2"));

                if (Physics.Raycast(t.position, t.TransformDirection(Vector3.down), out RaycastHit hit, _distance, ~(1 << 8)))
                {
                    Debug.DrawLine(t.position, hit.point, Color.green);
                    Gizmos.DrawWireSphere(hit.point, 0.1f);
                }
                else
                {
                    Debug.DrawRay(t.position, transform.TransformDirection(Vector3.down) * _distance, Color.red);
                    Gizmos.DrawWireSphere(t.position + (transform.TransformDirection(Vector3.down) * _distance), 0.1f);
                }
            }
        }
    }
}