// © 2025 Nanodogs Studios. All rights reserved.

using UnityEngine;

namespace Nanodogs.UniversalScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonPlayerMovement : MonoBehaviour
    {
        public float speed = 5f;

        private Rigidbody rb;
        private Vector3 inputDir;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true; // prevent tipping
        }

        private void Update()
        {
            // Get input in Update for snappy response
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            inputDir = (transform.right * x + transform.forward * z).normalized;
        }

        private void FixedUpdate()
        {
            // Move Rigidbody smoothly
            Vector3 targetVelocity = inputDir * speed;
            targetVelocity.y = rb.linearVelocity.y; // preserve gravity
            rb.linearVelocity = targetVelocity;
        }
    }
}
