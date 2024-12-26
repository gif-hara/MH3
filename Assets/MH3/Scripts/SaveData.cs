using System;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class SaveData
    {
        public const string Path = "SaveData.dat";

        [SerializeField]
        private UserData userData = new();
        public UserData UserData => userData;

        [SerializeField]
        private SystemData systemData = new();
        public SystemData SystemData => systemData;
    }
}
