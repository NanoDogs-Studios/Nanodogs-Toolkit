// © 2025 Nanodogs Studios. All rights reserved.

using UnityEngine;
using UnityEngine.InputSystem;

namespace Nanodogs.UniversalScripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonPlayerMovement : NanoMovementBase
    {
        [Header("Input Actions")]
        public InputActionReference moveAction;
        public InputActionReference jumpAction;

        private Vector3 inputDir;

        [Header("Ladder Settings")]
        public bool onLadder = false;
        public float ladderClimbSpeed = 5f;
        public float ladderStickStrength = 10f;
        public float ladderPushOffForce = 6f;
        private Vector3 ladderForward;
        private Vector3 ladderCenter;

        private void OnEnable()
        {
            if (moveAction != null) moveAction.action.Enable();
            if (jumpAction != null) jumpAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.action.Disable();
            if (jumpAction != null) jumpAction.action.Disable();
        }

        private void Update()
        {
            // Ensure input actions are active
            if (moveAction == null || jumpAction == null) return;

            // Read movement input
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            inputDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
            if (inputDir.magnitude > 1f) inputDir.Normalize();

            // Handle jump on ground
            if (jumpAction.action.WasPressedThisFrame() && IsGrounded() && !onLadder)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }

            // Ladder movement
            if (onLadder)
            {
                float verticalInput = moveInput.y;
                Vector3 climbVelocity = Vector3.up * verticalInput * ladderClimbSpeed;
                rb.linearVelocity = climbVelocity;

                // Stick player to ladder center
                Vector3 toCenter = ladderCenter - transform.position;
                toCenter.y = 0f;
                rb.AddForce(toCenter * ladderStickStrength, ForceMode.Acceleration);

                // Push off ladder
                if (jumpAction.action.WasPressedThisFrame())
                {
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