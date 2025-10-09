// © 2025 Nanodogs Studios. All rights reserved.

using UnityEngine;

namespace Nanodogs.UniversalScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonPlayerMovement : NanoMovementBase
    {
        private Vector3 inputDir;

        [Header("Ladder Settings")]
        public bool onLadder = false;
        public float ladderClimbSpeed = 5f;
        public float ladderStickStrength = 10f;
        public float ladderPushOffForce = 6f;
        private Vector3 ladderForward;
        private Vector3 ladderCenter;

        private void Update()
        {
            // Basic input
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            inputDir = (transform.right * x + transform.forward * z).normalized;
            if (inputDir.magnitude > 1f) inputDir.Normalize();

            // Normal jump
            if (Input.GetButtonDown("Jump") && IsGrounded() && !onLadder)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }

            if (onLadder)
            {
                // Climb up/down
                float verticalInput = Input.GetAxisRaw("Vertical");
                Vector3 climbVelocity = Vector3.up * verticalInput * ladderClimbSpeed;
                rb.linearVelocity = climbVelocity;

                // Stick player to ladder center
                Vector3 toCenter = (ladderCenter - transform.position);
                toCenter.y = 0f; // only keep horizontal axis
                rb.AddForce(toCenter * ladderStickStrength, ForceMode.Acceleration);

                // Push off ladder
                if (Input.GetButtonDown("Jump"))
                {
                    // get direction opposite to ladder forward
                    Vector3 pushDir = -ladderForward.normalized;
                    rb.useGravity = true;
                    onLadder = false;
                    rb.AddForce(pushDir * ladderPushOffForce + Vector3.up * 2f, ForceMode.VelocityChange);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!onLadder)
            {
                Vector3 targetVelocity = inputDir * speed;
                targetVelocity.y = rb.linearVelocity.y;
                rb.linearVelocity = targetVelocity;
            }
        }

        public void SetLadderData(Vector3 forward, Vector3 center)
        {
            ladderForward = forward;
            ladderCenter = center;
        }
    }
}
