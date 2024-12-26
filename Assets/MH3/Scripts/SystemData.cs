using System;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class SystemData
    {
        [SerializeField]
        private float masterVolume = 1.0f;

        [SerializeField]
        private float bgmVolume = 1.0f;

        [SerializeField]
        private float sfxVolume = 1.0f;

        public float MasterVolume
        {
            get => masterVolume;
            set => masterVolume = value;
        }

        public float BgmVolume
        {
            get => bgmVolume;
            set => bgmVolume = value;
        }

        public float SfxVolume
        {
            get => sfxVolume;
            set => sfxVolume = value;
        }
    }
}
