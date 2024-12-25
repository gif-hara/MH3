using System;
using System.Collections.Generic;
using HK;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class InstanceArmor : IReward
    {
        [SerializeField]
        private int instanceId;
        public int InstanceId => instanceId;

        [SerializeField]
        private int armorId;
        public int ArmorId => armorId;

        [SerializeField]
        private int defense;
        public int Defense => defense;

        [SerializeField]
        private Define.RareType defenseRareType;
        public Define.RareType DefenseRareType => defenseRareType;
        
        [SerializeField]
        private List<InstanceSkill> skills;
        public List<InstanceSkill> Skills => skills;
        
        public MasterData.ArmorSpec ArmorSpec => TinyServiceLocator.Resolve<MasterData>().ArmorSpecs.Get(armorId);

        public InstanceArmor(
            int instanceId,
            int armorId,
            int defense,
            Define.RareType defenseRareType,
            List<InstanceSkill> skills
        )
        {
            this.instanceId = instanceId;
            this.armorId = armorId;
            this.defense = defense;
            this.defenseRareType = defenseRareType;
            this.skills = skills;
        }
    }
}
