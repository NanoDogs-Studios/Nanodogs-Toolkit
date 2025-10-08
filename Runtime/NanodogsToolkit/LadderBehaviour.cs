using UnityEngine;

namespace Nanodogs.UniversalScripts
{
    /// <summary>
    /// LadderBehaviour is a placeholder class that inherits from NanoTrigger.
    /// It can be extended to implement ladder-specific functionality in the future.
    /// </summary>
    public class LadderBehaviour : NanoTrigger
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                Debug.Log("Player got on the ladder");
                other.attachedRigidbody.useGravity = false;

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player got off the ladder");
                other.attachedRigidbody.useGravity = true;

            }
        }
    }
}
