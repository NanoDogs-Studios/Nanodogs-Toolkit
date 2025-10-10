// © 2025 Nanodogs Studios. All rights reserved.

using UnityEngine;
using UnityEngine.InputSystem;

namespace Nanodogs.UniversalScripts
{
    public class FirstPersonPlayerCamera : MonoBehaviour
    {
        [Header("Input")]
        public InputActionReference lookAction;

        [Header("Settings")]
        public float mouseSensitivity = 100f;
        public float gamepadSensitivity = 200f;
        public float smoothTime = 0.05f;
        public Transform playerBody;

        private float xRotation = 0f;

        // Smooth values
        private Vector2 currentMouseDelta;
        private Vector2 currentMouseDeltaVelocity;

        private void OnEnable()
        {
            if (lookAction != null) lookAction.action.Enable();
        }

        private void OnDisable()
        {
            if (lookAction != null) lookAction.action.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (lookAction == null) return;

            // Determine which device is being used
            float sensitivity = mouseSensitivity;
            var lastControl = lookAction.action.activeControl;
            if (lastControl != null)
            {
                if (lastControl.device is Gamepad)
                    sensitivity = gamepadSensitivity;
                else if (lastControl.device is Mouse)
                    sensitivity = mouseSensitivity;
            }

            // Read raw look input
            Vector2 targetMouseDelta = lookAction.action.ReadValue<Vector2>() * sensitivity * Time.deltaTime;

            // Smooth the input
            currentMouseDelta = Vector2.SmoothDamp(
                currentMouseDelta,
                targetMouseDelta,
                ref currentMouseDeltaVelocity,
                smoothTime
            );

            xRotation -= currentMouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * currentMouseDelta.x);
        }
    }
}