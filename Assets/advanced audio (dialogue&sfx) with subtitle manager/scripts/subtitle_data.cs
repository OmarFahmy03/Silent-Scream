using System;
using UnityEngine;

namespace audio_subtitle_system
{
    [Serializable]
    public class KeyValueItem
    {
        public string key;
        public string value;
        public float duration = 3f; // Default duration in seconds
    }

    [Serializable]
    public class DataWrapper
    {
        public KeyValueItem[] items = {};
    }
}