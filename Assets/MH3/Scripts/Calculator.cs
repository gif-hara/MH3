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
            bool targetIsSuccessGuard
            )
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var damage = Mathf.FloorToInt(attacker.SpecController.Attack.CurrentValue * attackSpec.Power / 100.0f);
            damage = Mathf.FloorToInt(damage * (1.0f - target.SpecController.PhysicalDamageCutRate.CurrentValue));
            var flinchDamage = attackSpec.FlinchDamage;
            if (targetIsSuccessGuard)
            {
                damage = Mathf.FloorToInt(damage * gameRules.GuardSuccessDamageRate);
                flinchDamage = 0;
            }
            return new DamageData(damage, flinchDamage);
        }
    }
}
