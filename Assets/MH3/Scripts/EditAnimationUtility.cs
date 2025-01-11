#if UNITY_EDITOR
using System.Collections.Generic;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.Editor
{
    public class EditAnimationUtility : MonoBehaviour
    {
        public LocatorHolder locatorHolder;

        public List<WeaponModelData> weaponModelData = new List<WeaponModelData>();
    }
}
#endif
