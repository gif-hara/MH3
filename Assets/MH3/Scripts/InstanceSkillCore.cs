using System;
using System.Collections.Generic;
using HK;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class InstanceSkillCore : IReward
    {
        [SerializeField]
        private int instanceId;
        public int InstanceId => instanceId;

        [SerializeField]
        private int skillCoreSpecId;
        public int SkillCoreSpecId => skillCoreSpecId;

        [SerializeField]
        private int slot;
        public int Slot => slot;

        [SerializeField]
        private List<InstanceSkill> skills;
        public List<InstanceSkill> Skills => skills;

        public MasterData.SkillCoreSpec SkillCoreSpec => TinyServiceLocator.Resolve<MasterData>().SkillCoreSpecs.Get(skillCoreSpecId);

        public InstanceSkillCore(
            int instanceId,
            int skillCoreSpecId,
            int slot,
            List<InstanceSkill> skills
            )
        {
            this.instanceId = instanceId;
            this.skillCoreSpecId = skillCoreSpecId;
            this.slot = slot;
            this.skills = skills;
        }
    }
}
