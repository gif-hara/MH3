using System;
using System.Collections.Generic;
using HK;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponData
    {
        [SerializeField]
        private int instanceId;
        public int InstanceId => instanceId;

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

        [SerializeField]
        private List<int> instanceSkillCoreIds = new();
        public List<int> InstanceSkillCoreIds => instanceSkillCoreIds;

        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId);

        public InstanceWeaponData(
            int instanceId,
            int weaponId,
            int attack,
            Define.RareType attackRareType,
            float critical,
            Define.RareType criticalRareType,
            int skillSlot,
            Define.RareType skillSlotRareType
            )
        {
            this.instanceId = instanceId;
            this.weaponId = weaponId;
            this.attack = attack;
            this.attackRareType = attackRareType;
            this.critical = critical;
            this.criticalRareType = criticalRareType;
            this.skillSlot = skillSlot;
            this.skillSlotRareType = skillSlotRareType;
        }

        public void AddInstanceSkillCoreId(int instanceSkillCoreId)
        {
            instanceSkillCoreIds.Add(instanceSkillCoreId);
        }
    }
}
