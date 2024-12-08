using System;
using HK;
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

        [SerializeField]
        private Define.RareType attackRareType;
        public Define.RareType AttackRareType => attackRareType;

        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId);

        public InstanceWeaponData(int weaponId, int attack, Define.RareType attackRareType)
        {
            this.weaponId = weaponId;
            this.attack = attack;
            this.attackRareType = attackRareType;
        }
    }
}
