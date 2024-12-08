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

        [SerializeField]
        private float critical;
        public float Critical => critical;

        [SerializeField]
        private Define.RareType criticalRareType;
        public Define.RareType CriticalRareType => criticalRareType;

        [SerializeField]
        private int skillSlot;
        public int SkillSlot => skillSlot;

        [SerializeField]
        private Define.RareType skillSlotRareType;
        public Define.RareType SkillSlotRareType => skillSlotRareType;

        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId);

        public InstanceWeaponData(
            int weaponId,
            int attack,
            Define.RareType attackRareType,
            float critical,
            Define.RareType criticalRareType,
            int skillSlot,
            Define.RareType skillSlotRareType
            )
        {
            this.weaponId = weaponId;
            this.attack = attack;
            this.attackRareType = attackRareType;
            this.critical = critical;
            this.criticalRareType = criticalRareType;
            this.skillSlot = skillSlot;
            this.skillSlotRareType = skillSlotRareType;
        }
    }
}
