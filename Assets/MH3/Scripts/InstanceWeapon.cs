using System;
using System.Collections.Generic;
using System.Linq;
using HK;
using UnityEngine;
using UnityEngine.Serialization;

namespace MH3
{
    [Serializable]
    public class InstanceWeapon : IReward
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
        private Define.AbnormalStatusType abnormalStatusType;
        public Define.AbnormalStatusType AbnormalStatusType => abnormalStatusType;

        [SerializeField]
        private int abnormalStatusAttack;
        public int AbnormalStatusAttack => abnormalStatusAttack;

        [SerializeField]
        private Define.RareType abnormalStatusAttackRareType;
        public Define.RareType AbnormalStatusAttackRareType => abnormalStatusAttackRareType;

        [SerializeField]
        private Define.ElementType elementType;
        public Define.ElementType ElementType => elementType;

        [SerializeField]
        private int elementAttack;
        public int ElementAttack => elementAttack;

        [SerializeField]
        private Define.RareType elementAttackRareType;
        public Define.RareType ElementAttackRareType => elementAttackRareType;

        [SerializeField]
        private List<int> instanceSkillCoreIds = new();
        public List<int> InstanceSkillCoreIds => instanceSkillCoreIds;

        public int GetUsingSlotCount(List<InstanceSkillCore> instanceSkillCores) => instanceSkillCoreIds.Sum(x => instanceSkillCores.Find(y => y.InstanceId == x).Slot);

        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId);
        
        public IEnumerable<InstanceSkillCore> InstanceSkillCores => TinyServiceLocator.Resolve<UserData>().InstanceSkillCores.Where(x => instanceSkillCoreIds.Contains(x.InstanceId));

        public InstanceWeapon(
            int instanceId,
            int weaponId,
            int attack,
            Define.RareType attackRareType,
            float critical,
            Define.RareType criticalRareType,
            int skillSlot,
            Define.RareType skillSlotRareType,
            Define.AbnormalStatusType abnormalStatusType,
            int abnormalStatusAttack,
            Define.RareType abnormalStatusAttackRareType,
            Define.ElementType elementType,
            int elementAttack,
            Define.RareType elementAttackRareType
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
            this.abnormalStatusType = abnormalStatusType;
            this.abnormalStatusAttack = abnormalStatusAttack;
            this.abnormalStatusAttackRareType = abnormalStatusAttackRareType;
            this.elementType = elementType;
            this.elementAttack = elementAttack;
            this.elementAttackRareType = elementAttackRareType;
        }

        public void AddInstanceSkillCoreId(int instanceSkillCoreId)
        {
            instanceSkillCoreIds.Add(instanceSkillCoreId);
        }

        public void RemoveInstanceSkillCoreId(int instanceSkillCoreId)
        {
            instanceSkillCoreIds.Remove(instanceSkillCoreId);
        }
    }
}
