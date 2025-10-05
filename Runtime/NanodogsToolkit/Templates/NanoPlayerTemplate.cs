using UnityEngine;

namespace Nanodogs.UniversalScripts
{
    public class NanoPlayerTemplate : MonoBehaviour
    {
        public Transform cameraTransform;
        public Transform playerBodyTransform;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}