using System.Linq;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public static class Calculator
    {
        public static DamageData GetDefaultDamage(
            Actor attacker,
            Actor target,
            MasterData.AttackSpec attackSpec,
            Define.GuardResult targetGuardResult,
            Vector3 impactPosition
            )
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var attack = attacker.SpecController.GetAttackValue(attackSpec.ElementType);
            var damage = Mathf.FloorToInt(attack * attackSpec.Power / 100.0f);
            var isCritical = attackSpec.CanCritical && attacker.SpecController.CriticalTotal > Random.value;
            if (isCritical)
            {
                damage = Mathf.FloorToInt(damage * (gameRules.CriticalDamageRate + attacker.SpecController.CriticalDamageRate.Value));
            }
            if (target.SpecController.ContainsAppliedAbnormalStatus(Define.AbnormalStatusType.Collapse))
            {
                damage = Mathf.FloorToInt(damage * gameRules.CollapseDamageRate);
            }

            damage = Mathf.FloorToInt(damage * (1.0f - (float)target.SpecController.DefenseTotal / gameRules.DefenseRate));
            damage = Mathf.FloorToInt(damage * (1.0f - target.SpecController.GetCutRate(attackSpec.ElementType)));
            var consumedSuperArmor = false;
            if (target.SpecController.SuperArmorCount.CurrentValue > 0)
            {
                damage = Mathf.FloorToInt(damage * gameRules.SuperArmorDamageRate);
                consumedSuperArmor = true;
            }
            damage = Mathf.Max(1, damage);
            var flinchDamage = Mathf.FloorToInt(attackSpec.FlinchDamage + attackSpec.FlinchDamage * attacker.SpecController.FlinchDamageRate.Value);
            if (targetGuardResult == Define.GuardResult.SuccessGuard)
            {
                damage = Mathf.FloorToInt(damage * gameRules.GuardSuccessDamageRate);
                flinchDamage = 0;
            }
            else if (targetGuardResult == Define.GuardResult.SuccessJustGuard)
            {
                damage = 0;
                flinchDamage = 0;
            }
            return new DamageData(damage, flinchDamage, impactPosition, isCritical, targetGuardResult, consumedSuperArmor, attackSpec.IsStrong);
        }
    }
}
