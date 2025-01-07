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
                Define.SkillType.AttackUp => new AttackUp(level),
                Define.SkillType.CriticalUp => new CriticalUp(level),
                Define.SkillType.DefenseUp => new DefenseUp(level),
                Define.SkillType.PoisonElementAttackUp => new PoisonElementAttackUp(level),
                Define.SkillType.ParalysisElementAttackUp => new ParalysisElementAttackUp(level),
                Define.SkillType.CollapseElementAttackUp => new CollapseElementAttackUp(level),
                Define.SkillType.FireElementAttackUp => new FireElementAttackUp(level),
                Define.SkillType.WaterElementAttackUp => new WaterElementAttackUp(level),
                Define.SkillType.GrassElementAttackUp => new GrassElementAttackUp(level),
                Define.SkillType.HitPointMaxUp => new HitPointMaxUp(level),
                Define.SkillType.RecoveryCommandCountUp => new RecoveryCommandCountUp(level),
                Define.SkillType.RewardUp => new RewardUp(level),
                Define.SkillType.FlinchDamageUp => new FlinchDamageUp(level),
                Define.SkillType.RecoveryAmountUp => new RecoveryAmountUp(level),
                Define.SkillType.SuccessJustGuardCriticalUp => new SuccessJustGuardCriticalUp(level),
                Define.SkillType.LastComboAttackUp => new LastComboAttackUp(level),
                Define.SkillType.StaminaMaxUp => new StaminaMaxUp(level),
                Define.SkillType.InvokeSharpenAttackUp => new InvokeSharpenAttackUp(level),
                Define.SkillType.AttackUpForSuperArmor => new AttackUpForSuperArmor(level),
                Define.SkillType.StaminaRecoveryAmountUp => new StaminaRecoveryAmountUp(level),
                Define.SkillType.RecoveryStaminaForCritical => new RecoveryStaminaForCritical(level),
                Define.SkillType.RecoveryHitPointForAttack => new RecoveryHitPointForAttack(level),
                _ => null
            };
        }
    }
}
