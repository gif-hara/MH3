using System;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponData
    {
        [SerializeField]
        private int weaponId;
        public int WeaponId => weaponId;

        [SerializeField]
        private int attack;
        public int Attack => attack;

        public InstanceWeaponData(int weaponId, int attack)
        {
            this.weaponId = weaponId;
            this.attack = attack;
        }
    }
}
