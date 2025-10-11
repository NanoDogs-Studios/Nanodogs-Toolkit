// © 2025 Nanodogs Studios. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace Nanodogs.UniversalScripts
{
    public class NanoTrigger : MonoBehaviour
    {
        public UnityAction onTriggerEnter;
        public UnityAction onTriggerExit;
        public UnityAction onTriggerStay;   

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke();
        }
        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke();
        }
        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke();
        }
    }
}