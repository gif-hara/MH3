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
            return skillType switch
            {
                Define.SkillType.AttackUp => new Skill(level)
                    .RegisterParameterSelector(Define.ActorParameterType.Attack, _ => TinyServiceLocator.Resolve<MasterData>().SkillAttackUp.Get(level).Value),
                _ => throw new System.NotImplementedException($"SkillType {skillType} is not implemented."),
            };
        }
    }
}
