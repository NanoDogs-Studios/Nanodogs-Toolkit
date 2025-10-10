using UnityEngine;
using UnityEngine.InputSystem;

namespace Nanodogs.UniversalScripts
{
    public class NanoPlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [Tooltip("Key used to interact with objects.")]
        public InputActionReference interactKey;

        [Tooltip("Maximum distance to interactable objects.")]
        public float interactDistance = 3f;

        [Tooltip("Layer mask for interactable objects.")]
        public LayerMask interactableLayer;

        [Tooltip("Camera used for raycasting.")]
        public Camera cam;

        private void Update()
        {
            CheckForInteraction();
        }

        private void CheckForInteraction()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
            {
                NanoInteractable interactable = hit.collider.GetComponent<NanoInteractable>();
                if (interactable != null && interactable.isCurrentlyInteractable)
                {
                    // Optional: highlight object here

                    if (interactKey != null && interactKey.action.WasPerformedThisFrame())
                    {
                        interactable.Interact();
                    }
                }
            }
        }
    }
}
