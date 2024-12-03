using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
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

        private readonly ReactiveProperty<int> attack = new(0);

        private readonly ReactiveProperty<float> physicalDamageCutRate = new(0.0f);

        private readonly ReactiveProperty<int> weaponId = new(0);

        private readonly ReactiveProperty<int> flinch = new(0);

        public readonly ReactiveProperty<bool> CanAddFlinchDamage = new(true);

        public readonly ReactiveProperty<bool> Invincible = new(false);

        public readonly List<string> ComboAnimationKeys = new();

        public string JustGuardAttackAnimationKey { get; private set; }

        public ActorSpecController(Actor actor, MasterData.ActorSpec spec)
        {
            this.actor = actor;
            this.spec = spec;
            hitPoint.Value = spec.HitPoint;
            attack.Value = spec.Attack;
            physicalDamageCutRate.Value = spec.PhysicalDamageCutRate;
            weaponId.Value = spec.WeaponId;
            ComboAnimationKeys.Clear();
            foreach (var combo in WeaponSpec.GetCombos())
            {
                ComboAnimationKeys.Add(combo.AnimationKey);
            }
            JustGuardAttackAnimationKey = WeaponSpec.JustGuardAttackAnimationKey;
        }

        public Define.ActorType ActorType => spec.ActorType;

        public ReadOnlyReactiveProperty<int> HitPoint => hitPoint;

        public ReadOnlyReactiveProperty<int> Attack => attack;

        public ReadOnlyReactiveProperty<float> PhysicalDamageCutRate => physicalDamageCutRate;

        public float MoveSpeed => spec.MoveSpeed;

        public float RotationSpeed => spec.RotationSpeed;

        public ReadOnlyReactiveProperty<int> WeaponId => weaponId;

        public ReadOnlyReactiveProperty<int> Flinch => flinch;

        public ScriptableSequences AttackSequences => spec.AttackSequences;

        public ScriptableSequences FlinchSequences => spec.FlinchSequences;

        public ScriptableSequences DodgeSequences => spec.DodgeSequences;

        public ScriptableSequences GuardSequences => spec.GuardSequences;

        public ScriptableSequences SuccessJustGuardSequences => spec.SuccessJustGuardSequences;

        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId.Value);

        public void TakeDamage(Actor attacker, MasterData.AttackSpec attackSpec)
        {
            if (attackSpec == null)
            {
                Debug.LogError("AttackSpec is null.");
                return;
            }
            if (actor.SpecController.Invincible.Value)
            {
                return;
            }

#if DEBUG
            if (actor.SpecController.ActorType == Define.ActorType.Player &&
                TinyServiceLocator.Resolve<GameDebugData>().InvinciblePlayer)
            {
                return;
            }
            else if (actor.SpecController.ActorType != Define.ActorType.Player &&
                     TinyServiceLocator.Resolve<GameDebugData>().InvincibleEnemy)
            {
                return;
            }
#endif

            var guardResult = actor.GuardController.GetGuardResult(attacker.transform.position);
            attacker.TimeController.BeginHitStopAsync(attackSpec.HitStopTimeScaleActor, attackSpec.HitStopDurationActor).Forget();
            actor.TimeController.BeginHitStopAsync(attackSpec.HitStopTimeScaleTarget, attackSpec.HitStopDurationTarget).Forget();
            LMotion.Shake.Create(Vector3.zero, Vector3.one * attackSpec.ShakeStrength, attackSpec.ShakeDuration)
                .WithFrequency(attackSpec.ShakeFrequency)
                .WithDampingRatio(attackSpec.ShakeDampingRatio)
                .BindToLocalPosition(actor.LocatorHolder.Get("Shake"))
                .AddTo(actor);

            if (guardResult == ActorGuardController.GuardResult.SuccessJustGuard)
            {
                actor.StateMachine.TryChangeState(actor.SpecController.SuccessJustGuardSequences, force: true);
            }
            else
            {
                var damageData = Calculator.GetDefaultDamage(attacker, actor, attackSpec, guardResult);
#if DEBUG
                if (actor.SpecController.ActorType == Define.ActorType.Player && TinyServiceLocator.Resolve<GameDebugData>().DamageZeroPlayer)
                {
                    damageData.Damage = 0;
                }
                else if (actor.SpecController.ActorType == Define.ActorType.Enemy && TinyServiceLocator.Resolve<GameDebugData>().DamageZeroEnemy)
                {
                    damageData.Damage = 0;
                }
#endif
                var fixedHitPoint = hitPoint.Value - damageData.Damage;
                fixedHitPoint = fixedHitPoint < 0 ? 0 : fixedHitPoint;
                hitPoint.Value = fixedHitPoint;
                if (CanAddFlinchDamage.Value)
                {
                    flinch.Value += damageData.FlinchDamage;
                }

                if (fixedHitPoint <= 0)
                {
                    Object.Destroy(actor.gameObject);
                }
                if (actor.SpecController.CanPlayFlinch())
                {
                    var lookAt = attacker.transform.position - actor.transform.position;
                    lookAt.y = 0.0f;
                    actor.MovementController.RotateImmediate(Quaternion.LookRotation(lookAt));
                    actor.MovementController.CanRotate.Value = false;
                    actor.StateMachine.TryChangeState(actor.SpecController.FlinchSequences, force: true, containerAction: c => c.Register("FlinchName", attackSpec.FlinchName));
                    actor.SpecController.ResetFlinch();
                }
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
