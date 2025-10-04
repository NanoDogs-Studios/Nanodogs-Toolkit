using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanodogs.UniversalScripts
{
    public class LevelLoadTrigger : MonoBehaviour
    {
        public string levelName;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // TODO: use a load manager script to handle loading screens, transitions, etc.
                Debug.Log("Loading level: " + levelName);
            }
        }
    }
}