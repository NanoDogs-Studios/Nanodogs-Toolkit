using UnityEngine;

namespace Nanodogs.API.NanoMusic
{
    public class NanoMusicAsset : ScriptableObject
    {
        public AudioClip musicClip;
        public bool loop = true;
        public float volume = 1.0f;
        public float pitch = 1.0f;
        public bool playOnAwake = false;
    }
}