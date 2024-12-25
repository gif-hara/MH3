using System.Collections.Generic;
using HK;

namespace MH3
{
    public static class InstanceArmorFactory
    {
        public static InstanceArmor Create(UserData userData, int armorSpecId)
        {
            var armorSpec = TinyServiceLocator.Resolve<MasterData>().ArmorSpecs.Get(armorSpecId);
            var instanceId = userData.GetAndIncrementCreatedInstanceArmorCount();
            var defenseSpec = armorSpec.GetDefenses().Lottery(x => x.Weight);
            var skills = new List<InstanceSkill>();
            var skillCount = armorSpec.GetSkillCounts().Lottery(x => x.Weight);
            if (skillCount != null)
            {
                for (var i = 0; i < skillCount.Count; i++)
                {
                    var armorSkill = armorSpec.GetSkills().Lottery(x => x.Weight);
                    var instanceSkill = new InstanceSkill(armorSkill.SkillType, armorSkill.Level, armorSkill.RareType);
                    skills.Add(instanceSkill);
                }
            }
            return new InstanceArmor(
                instanceId,
                armorSpecId,
                defenseSpec?.Defense ?? 0,
                defenseSpec?.RareType ?? Define.RareType.Common,
                skills
                );
        }
    }
}
