using System.Collections.Generic;
using HK;
using R3;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorSpecController
    {
        private readonly Actor actor;

        private readonly MasterData.ActorSpec spec;

        private readonly ReactiveProperty<int> hitPoint = new(0);

        private readonly ReactiveProperty<int> weaponId = new(0);

        private readonly ReactiveProperty<int> flinch = new(0);

        public readonly ReactiveProperty<bool> CanAddFlinchDamage = new(true);

        public readonly ReactiveProperty<bool> Invincible = new(false);
        
        public readonly List<string> ComboAnimationKeys = new();

        public ActorSpecController(Actor actor, MasterData.ActorSpec spec)
        {
            this.actor = actor;
            this.spec = spec;
            hitPoint.Value = spec.HitPoint;
            weaponId.Value = spec.WeaponId;
            ComboAnimationKeys.Clear();
            foreach (var combo in WeaponSpec.GetCombos())
            {
                ComboAnimationKeys.Add(combo.AnimationKey);
            }
        }

        public ReadOnlyReactiveProperty<int> HitPoint => hitPoint;

        public float MoveSpeed => spec.MoveSpeed;

        public float RotationSpeed => spec.RotationSpeed;

        public ReadOnlyReactiveProperty<int> WeaponId => weaponId;

        public ReadOnlyReactiveProperty<int> Flinch => flinch;

        public ScriptableSequences AttackSequences => spec.AttackSequences;

        public ScriptableSequences FlinchSequences => spec.FlinchSequences;

        public ScriptableSequences DodgeSequences => spec.DodgeSequences;

        public ScriptableSequences GuardSequences => spec.GuardSequences;
        
        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId.Value);

        public void TakeDamage(DamageData data)
        {
            var result = hitPoint.Value - data.Damage;
            result = result < 0 ? 0 : result;
            hitPoint.Value = result;
            if (CanAddFlinchDamage.Value)
            {
                flinch.Value += data.FlinchDamage;
            }

            if (result <= 0)
            {
                Object.Destroy(actor.gameObject);
            }
        }

        public bool CanPlayFlinch()
        {
            return flinch.Value >= spec.FlinchThreshold;
        }

        public void ResetFlinch()
        {
            flinch.Value = 0;
        }
    }
}
