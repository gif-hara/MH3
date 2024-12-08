using System.Collections.Generic;
using HK;
using UnityEngine;

namespace MH3
{
    public static class InstanceSkillCoreFactory
    {
        public static InstanceSkillCore Create(UserData userData, int skillCoreSpecId)
        {
            var skillCoreSpec = TinyServiceLocator.Resolve<MasterData>().SkillCoreSpecs.Get(skillCoreSpecId);
            var skillCoreCount = skillCoreSpec.GetSkillCoreCounts().Lottery(x => x.Weight);
            var skills = new List<InstanceSkill>();
            for (var i = 0; i < skillCoreCount.Count; i++)
            {
                var skillCoreEffect = skillCoreSpec.GetSkillCoreEffects().Lottery(x => x.Weight);
                skills.Add(new InstanceSkill(skillCoreEffect.SkillType, skillCoreEffect.Level, skillCoreEffect.RareType));
            }
            return new InstanceSkillCore(
                userData.GetAndIncrementCreatedInstanceSkillCoreCount(),
                skillCoreSpecId,
                skillCoreSpec.Slot,
                skills
                );
        }
    }
}
