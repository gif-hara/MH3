using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
using MH3.SkillSystems;
using R3;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorSpecController
    {
        private readonly Actor actor;

        private readonly MasterData.ActorSpec spec;

        private readonly ReactiveProperty<int> hitPointMax = new(0);

        private readonly ReactiveProperty<int> hitPoint = new(0);

        private readonly ReactiveProperty<int> attack = new(0);

        private readonly ReactiveProperty<int> attackInstanceWeapon = new(0);

        private readonly ReactiveProperty<float> criticalInstanceWeapon = new(0);

        private readonly ReactiveProperty<float> cutRatePhysicalDamage = new(0.0f);

        private readonly ReactiveProperty<int> weaponId = new(0);

        private readonly ReactiveProperty<int> flinch = new(0);

        public readonly ReactiveProperty<bool> CanAddFlinchDamage = new(true);

        public readonly ReactiveProperty<bool> Invincible = new(false);

        public readonly List<string> ComboAnimationKeys = new();

        public string JustGuardAttackAnimationKey { get; private set; }

        public string StrongAttackAnimationKey { get; private set; }

        public readonly ReactiveProperty<Actor> Target = new(null);

        private readonly ReactiveProperty<Define.FlinchType> flinchType = new(Define.FlinchType.None);

        private readonly List<ISkill> skills = new();

        private readonly Subject<Unit> onFlinch = new();

        private readonly Subject<DamageData> onTakeDamage = new();

        private readonly Subject<Unit> onDead = new();

        public ActorSpecController(Actor actor, MasterData.ActorSpec spec)
        {
            this.actor = actor;
            this.spec = spec;
            hitPointMax.Value = spec.HitPoint;
            hitPoint.Value = spec.HitPoint;
            attack.Value = spec.Attack;
            cutRatePhysicalDamage.Value = spec.PhysicalDamageCutRate;
            weaponId.Value = spec.WeaponId;
            ComboAnimationKeys.Clear();
            foreach (var combo in WeaponSpec.GetCombos())
            {
                ComboAnimationKeys.Add(combo.AnimationKey);
            }
            JustGuardAttackAnimationKey = WeaponSpec.JustGuardAttackAnimationKey;
            StrongAttackAnimationKey = WeaponSpec.StrongAttackAnimationKey;
        }

        public Define.ActorType ActorType => spec.ActorType;

        public ReadOnlyReactiveProperty<int> HitPointMax => hitPointMax;

        public ReadOnlyReactiveProperty<int> HitPoint => hitPoint;

        public ReadOnlyReactiveProperty<int> Attack => attack;

        public ReadOnlyReactiveProperty<int> AttackInstanceWeapon => attackInstanceWeapon;

        public int AttackTotal => attack.Value + attackInstanceWeapon.Value + skills.Sum(x => x.GetAttack());

        public float CriticalTotal => criticalInstanceWeapon.Value;

        public ReadOnlyReactiveProperty<float> CutRatePhysicalDamage => cutRatePhysicalDamage;

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

        public ReadOnlyReactiveProperty<Define.FlinchType> FlinchType => flinchType;

        public Observable<Unit> OnFlinch => onFlinch;

        public Observable<DamageData> OnTakeDamage => onTakeDamage;

        public Observable<Unit> OnDead => onDead;

        public void ChangeInstanceWeapon(InstanceWeapon instanceWeaponData, List<InstanceSkillCore> instanceSkillCores)
        {
            attackInstanceWeapon.Value = instanceWeaponData.Attack;
            criticalInstanceWeapon.Value = instanceWeaponData.Critical;
            weaponId.Value = instanceWeaponData.WeaponId;
            var instanceSkills = instanceWeaponData.InstanceSkillCoreIds
                .Select(x => instanceSkillCores.Find(y => y.InstanceId == x))
                .SelectMany(x => x.Skills);
            skills.Clear();
            skills.AddRange(SkillFactory.CreateSkills(instanceSkills));
        }

        public void TakeDamage(Actor attacker, MasterData.AttackSpec attackSpec, Vector3 impactPosition)
        {
            if (attackSpec == null)
            {
                Debug.LogError("AttackSpec is null.");
                return;
            }
            if (Invincible.Value)
            {
                return;
            }

#if DEBUG
            if (ActorType == Define.ActorType.Player &&
                TinyServiceLocator.Resolve<GameDebugData>().InvinciblePlayer)
            {
                return;
            }
            else if (ActorType != Define.ActorType.Player &&
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
                actor.StateMachine.TryChangeState(SuccessJustGuardSequences, force: true);
                var gameRules = TinyServiceLocator.Resolve<GameRules>();
                TinyServiceLocator.Resolve<AudioManager>().PlaySfx(gameRules.SuccessJustGuardSfxKey);
            }
            else
            {
                var damageData = Calculator.GetDefaultDamage(attacker, actor, attackSpec, guardResult, impactPosition);
                if (guardResult == ActorGuardController.GuardResult.SuccessGuard)
                {
                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx(TinyServiceLocator.Resolve<GameRules>().SuccessGuardSfxKey);
                }
                else
                {
                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx(attackSpec.HitSfxKey);
                }
                var hitEffect = TinyServiceLocator.Resolve<EffectManager>().Rent(attackSpec.HitEffectKey);
                hitEffect.transform.position = impactPosition;

                if (hitPoint.Value <= 0)
                {
                    return;
                }
                var fixedHitPoint = hitPoint.Value - damageData.Damage;
#if DEBUG
                if (ActorType == Define.ActorType.Player && TinyServiceLocator.Resolve<GameDebugData>().DamageZeroPlayer
                || ActorType == Define.ActorType.Enemy && TinyServiceLocator.Resolve<GameDebugData>().DamageZeroEnemy)
                {
                    fixedHitPoint = hitPoint.Value + damageData.Damage;
                }
#endif
                fixedHitPoint = fixedHitPoint < 0 ? 0 : fixedHitPoint;
                hitPoint.Value = fixedHitPoint;
                if (CanAddFlinchDamage.Value)
                {
                    flinch.Value += damageData.FlinchDamage;
                }

                if (fixedHitPoint <= 0)
                {
                    actor.StateMachine.TryChangeState(spec.DeadSequences, force: true);
                    onDead.OnNext(Unit.Default);
                }
                else if (CanPlayFlinch() || attackSpec.ForceFlinch)
                {
                    var lookAt = attacker.transform.position - actor.transform.position;
                    lookAt.y = 0.0f;
                    actor.MovementController.RotateImmediate(Quaternion.LookRotation(lookAt));
                    actor.MovementController.CanRotate.Value = false;
                    flinchType.Value = attackSpec.FlinchType;
                    actor.StateMachine.TryChangeState(FlinchSequences, force: true, containerAction: c => c.Register("FlinchName", attackSpec.FlinchName));
                    ResetFlinch();
                    onFlinch.OnNext(Unit.Default);
                }

                if (guardResult == ActorGuardController.GuardResult.SuccessGuard)
                {
                    actor.StateMachine.TryChangeState(spec.SuccessGuardSequences, force: true);
                }
                onTakeDamage.OnNext(damageData);
            }
        }

        public bool CanPlayFlinch()
        {
#if DEBUG
            var debugData = TinyServiceLocator.Resolve<GameDebugData>();
            if (ActorType == Define.ActorType.Player && debugData.ForceFlinchSmallPlayer
            || ActorType == Define.ActorType.Enemy && debugData.ForceFlinchSmallEnemy)
            {
                return true;
            }
#endif
            return flinch.Value >= spec.FlinchThreshold;
        }

        public void ResetFlinch()
        {
            flinch.Value = 0;
        }

        public void ResetFlinchType()
        {
            flinchType.Value = Define.FlinchType.None;
        }

        public void RecoveryFromAnimation()
        {
            var result = hitPoint.Value + TinyServiceLocator.Resolve<GameRules>().RecoveryAmount;
            hitPoint.Value = result > spec.HitPoint ? spec.HitPoint : result;
        }

        public void ResetAll()
        {
            hitPoint.Value = spec.HitPoint;
            flinch.Value = 0;
            flinchType.Value = Define.FlinchType.None;
            CanAddFlinchDamage.Value = true;
            Invincible.Value = false;
            actor.StateMachine.TryChangeState(spec.InitialStateSequences, force: true);
        }

        public bool TryRecovery()
        {
            if (hitPoint.Value >= spec.HitPoint)
            {
                return false;
            }
            return actor.StateMachine.TryChangeState(spec.RecoverySequences);
        }

#if DEBUG
        public void SetHitPointDebug(int value)
        {
            hitPoint.Value = value;
        }
#endif
    }
}
