using System.Collections.Generic;
using System.Linq;
using HK;

namespace MH3.SkillSystems
{
    public static class SkillFactory
    {
        public static List<ISkill> CreateSkills(IEnumerable<InstanceSkill> instanceSkills)
        {
            var skills = new List<ISkill>();
            foreach (var i in instanceSkills.GroupBy(x => x.SkillType))
            {
                var skill = CreateSkill(i.Key, i.Sum(x => x.Level));
                skills.Add(skill);
            }
            return skills;
        }

        private static ISkill CreateSkill(Define.SkillType skillType, int level)
        {
            var masterData = TinyServiceLocator.Resolve<MasterData>();
            var result = new Skill();
            foreach (var i in masterData.SkillTypeToParameters.Get(skillType))
            {
                result.RegisterParameterSelector(i.ActorParameterType, _ => i.SkillLevelValueType.GetSkillLevelValue().Get(level).Value);
            }

            return result;
        }
    }
}
