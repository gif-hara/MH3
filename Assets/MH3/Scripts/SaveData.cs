using System;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class SaveData
    {
        [SerializeField]
        private UserData userData = new();
        public UserData UserData => userData;
    }
}
