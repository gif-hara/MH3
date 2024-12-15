using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
using MH3.AbnormalStatusSystems;
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

        private readonly ReactiveProperty<float> cutRateFireDamage = new(0.0f);

        private readonly ReactiveProperty<float> cutRateWaterDamage = new(0.0f);

        private readonly ReactiveProperty<float> cutRateGrassDamage = new(0.0f);

        private readonly ReactiveProperty<int> abnormalStatusAttackInstanceWeapon = new(0);

        private readonly ReactiveProperty<Define.AbnormalStatusType> abnormalStatusAttackType = new(Define.AbnormalStatusType.None);

        private readonly ReactiveProperty<int> elementAttackInstanceWeapon = new(0);

        private readonly ReactiveProperty<Define.ElementType> elementAttackType = new(Define.ElementType.None);

        private readonly Dictionary<Define.AbnormalStatusType, ReactiveProperty<int>> abnormalStatusThreshold = new();

        private readonly Dictionary<Define.AbnormalStatusType, ReactiveProperty<int>> abnormalStatusValues = new();

        private readonly HashSet<Define.AbnormalStatusType> appliedAbnormalStatuses = new();

        private readonly ReactiveProperty<int> recoveryCommandCount = new(0);

        private readonly ReactiveProperty<int> rewardUp = new(0);

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

        private readonly CancellationTokenSource deadCancellationTokenSource = new();

        public ScriptableSequences GuardPerformedSequences { get; private set; }

        public ScriptableSequences GuardCanceledSequences { get; private set; }

        public ActorSpecController(Actor actor, MasterData.ActorSpec spec)
        {
            this.actor = actor;
            this.spec = spec;
            hitPointMax.Value = spec.HitPoint;
            hitPoint.Value = spec.HitPoint;
            attack.Value = spec.Attack;
            cutRatePhysicalDamage.Value = spec.PhysicalDamageCutRate;
            cutRateFireDamage.Value = spec.FireDamageCutRate;
            cutRateWaterDamage.Value = spec.WaterDamageCutRate;
            cutRateGrassDamage.Value = spec.GrassDamageCutRate;
            SetWeaponId(spec.WeaponId);
            recoveryCommandCount.Value = spec.RecoveryCommandCount;
            abnormalStatusThreshold.Add(Define.AbnormalStatusType.Poison, new ReactiveProperty<int>(spec.PoisonThreshold));
            abnormalStatusThreshold.Add(Define.AbnormalStatusType.Paralysis, new ReactiveProperty<int>(spec.ParalysisThreshold));
            abnormalStatusThreshold.Add(Define.AbnormalStatusType.Collapse, new ReactiveProperty<int>(spec.CollapseThreshold));
        }

        public Define.ActorType ActorType => spec.ActorType;

        public ReadOnlyReactiveProperty<int> HitPointMax => hitPointMax;

        public ReadOnlyReactiveProperty<int> HitPoint => hitPoint;

        public ReadOnlyReactiveProperty<int> Attack => attack;

        public ReadOnlyReactiveProperty<int> AttackInstanceWeapon => attackInstanceWeapon;

        public int AttackTotal => attack.Value + attackInstanceWeapon.Value + skills.Sum(x => x.GetParameterInt(Define.ActorParameterType.Attack, actor));

        public float CriticalTotal => criticalInstanceWeapon.Value + skills.Sum(x => x.GetParameter(Define.ActorParameterType.Critical, actor));

        public int AbnormalStatusAttackTotal => abnormalStatusAttackInstanceWeapon.Value + skills.Sum(x => x.GetParameterInt(AbnormalStatusAttackType.ToActorParameterType(), actor));

        public Define.AbnormalStatusType AbnormalStatusAttackType => abnormalStatusAttackType.Value;

        public int ElementAttackTotal => elementAttackInstanceWeapon.Value + skills.Sum(x => x.GetParameterInt(ElementAttackType.ToActorParameterType(), actor));

        public Define.ElementType ElementAttackType => elementAttackType.Value;

        public int DefenseTotal => skills.Sum(x => x.GetParameterInt(Define.ActorParameterType.Defense, actor));

        public ReadOnlyReactiveProperty<float> CutRatePhysicalDamage => cutRatePhysicalDamage;

        public ReadOnlyReactiveProperty<float> CutRateFireDamage => cutRateFireDamage;

        public ReadOnlyReactiveProperty<float> CutRateWaterDamage => cutRateWaterDamage;

        public ReadOnlyReactiveProperty<float> CutRateGrassDamage => cutRateGrassDamage;

        public ReadOnlyReactiveProperty<int> RecoveryCommandCount => recoveryCommandCount;

        public int PoisonDuration => spec.PoisonDuration;

        public int ParalysisDuration => spec.ParalysisDuration;

        public int CollapseDuration => spec.CollapseDuration;

        public float MoveSpeed => spec.MoveSpeed;

        public float RotationSpeed => spec.RotationSpeed;

        public bool VisibleStatusUI => spec.VisibleStatusUI;

        public bool IsDead => hitPoint.Value <= 0;

        public ReadOnlyReactiveProperty<int> WeaponId => weaponId;

        public ReadOnlyReactiveProperty<int> Flinch => flinch;

        public ReadOnlyReactiveProperty<int> RewardUp => rewardUp;

        public ScriptableSequences AttackStateSequences => spec.AttackSequences;

        public ScriptableSequences FlinchStateSequences => spec.FlinchSequences;

        public ScriptableSequences DodgeStateSequences => spec.DodgeSequences;

        public ScriptableSequences GuardStateSequences => spec.GuardSequences;

        public ScriptableSequences SuccessJustGuardSequences => spec.SuccessJustGuardSequences;

        public MasterData.WeaponSpec WeaponSpec => TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponId.Value);

        public ReadOnlyReactiveProperty<Define.FlinchType> FlinchType => flinchType;

        public Observable<Unit> OnFlinch => onFlinch;

        public Observable<DamageData> OnTakeDamage => onTakeDamage;

        public Observable<Unit> OnDead => onDead;

        public CancellationToken DeadCancellationToken => deadCancellationTokenSource.Token;

        public int GetAttackValue(Define.ElementType elementType)
        {
            return elementType switch
            {
                Define.ElementType.None => AttackTotal,
                Define.ElementType.Fire => ElementAttackTotal,
                Define.ElementType.Water => ElementAttackTotal,
                Define.ElementType.Grass => ElementAttackTotal,
                _ => throw new System.NotImplementedException($"未対応の属性です {elementType}"),
            };
        }

        public ReadOnlyReactiveProperty<float> GetCutRate(Define.ElementType elementType)
        {
            return elementType switch
            {
                Define.ElementType.None => CutRatePhysicalDamage,
                Define.ElementType.Fire => CutRateFireDamage,
                Define.ElementType.Water => CutRateWaterDamage,
                Define.ElementType.Grass => CutRateGrassDamage,
                _ => throw new System.NotImplementedException($"未対応の属性です {elementType}"),
            };
        }

        private void SetWeaponId(int value)
        {
            weaponId.Value = value;
            var weaponSpec = WeaponSpec;
            GuardPerformedSequences = weaponSpec.GuardPerformedSequences;
            GuardCanceledSequences = weaponSpec.GuardCanceledSequences;
            JustGuardAttackAnimationKey = weaponSpec.JustGuardAttackAnimationKey;
            StrongAttackAnimationKey = weaponSpec.StrongAttackAnimationKey;
            ComboAnimationKeys.Clear();
            foreach (var combo in weaponSpec.GetCombos())
            {
                ComboAnimationKeys.Add(combo.AnimationKey);
            }
        }

        public void ChangeInstanceWeapon(InstanceWeapon instanceWeaponData, List<InstanceSkillCore> instanceSkillCores)
        {
            SetWeaponId(instanceWeaponData.WeaponId);
            attackInstanceWeapon.Value = instanceWeaponData.Attack;
            criticalInstanceWeapon.Value = instanceWeaponData.Critical;
            abnormalStatusAttackInstanceWeapon.Value = instanceWeaponData.AbnormalStatusAttack;
            abnormalStatusAttackType.Value = instanceWeaponData.AbnormalStatusType;
            elementAttackInstanceWeapon.Value = instanceWeaponData.ElementAttack;
            elementAttackType.Value = instanceWeaponData.ElementType;
            var instanceSkills = instanceWeaponData.InstanceSkillCoreIds
                .Select(x => instanceSkillCores.Find(y => y.InstanceId == x))
                .SelectMany(x => x.Skills);
            skills.Clear();
            skills.AddRange(SkillFactory.CreateSkills(instanceSkills));
            hitPointMax.Value = spec.HitPoint + skills.Sum(x => x.GetParameterInt(Define.ActorParameterType.Health, actor));
            hitPoint.Value = hitPointMax.Value;
            recoveryCommandCount.Value = spec.RecoveryCommandCount + skills.Sum(x => x.GetParameterInt(Define.ActorParameterType.RecoveryCommandCount, actor));
            rewardUp.Value = skills.Sum(x => x.GetParameterInt(Define.ActorParameterType.Reward, actor));
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

            var guardResult = actor.ActionController.GetGuardResult(attacker.transform.position);
            attacker.TimeController.BeginHitStopAsync(attackSpec.HitStopTimeScaleActor, attackSpec.HitStopDurationActor).Forget();
            actor.TimeController.BeginHitStopAsync(attackSpec.HitStopTimeScaleTarget, attackSpec.HitStopDurationTarget).Forget();
            LMotion.Shake.Create(Vector3.zero, Vector3.one * attackSpec.ShakeStrength, attackSpec.ShakeDuration)
                .WithFrequency(attackSpec.ShakeFrequency)
                .WithDampingRatio(attackSpec.ShakeDampingRatio)
                .BindToLocalPosition(actor.LocatorHolder.Get("Shake"))
                .AddTo(actor);
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var masterData = TinyServiceLocator.Resolve<MasterData>();

            if (guardResult == Define.GuardResult.SuccessJustGuard)
            {
                actor.StateMachine.TryChangeState(SuccessJustGuardSequences, force: true);
                TinyServiceLocator.Resolve<AudioManager>().PlaySfx(gameRules.SuccessJustGuardSfxKey);
            }
            else
            {
                var damageData = Calculator.GetDefaultDamage(attacker, actor, attackSpec, guardResult, impactPosition);
                if (guardResult == Define.GuardResult.SuccessGuard)
                {
                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx(TinyServiceLocator.Resolve<GameRules>().SuccessGuardSfxKey);
                }
                else
                {
                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx(attackSpec.HitSfxKey);
                }
                var hitEffect = TinyServiceLocator.Resolve<EffectManager>().Rent(attackSpec.HitEffectKey);
                hitEffect.transform.position = impactPosition;

                TinyServiceLocator.Resolve<GameCameraController>().BeginImpulseSource(attackSpec.HitCameraImpulseSourceKey);

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

                var abnormalStatustype = attacker.SpecController.AbnormalStatusAttackType;
                if (abnormalStatustype != Define.AbnormalStatusType.None && !appliedAbnormalStatuses.Contains(abnormalStatustype) && !IsDead)
                {
                    if (!abnormalStatusValues.TryGetValue(abnormalStatustype, out var value))
                    {
                        value = new ReactiveProperty<int>(0);
                        abnormalStatusValues.Add(abnormalStatustype, value);
                    }
                    value.Value += Mathf.FloorToInt(attacker.SpecController.AbnormalStatusAttackTotal * attackSpec.AbnormalStatusPower);
                    if (value.Value >= abnormalStatusThreshold[abnormalStatustype].Value)
                    {
                        appliedAbnormalStatuses.Add(abnormalStatustype);
                        var abnormalStatus = AbnormalStatusFactory.Create(abnormalStatustype);
                        abnormalStatus.Apply(actor);
                        value.Value = 0;
                    }
                }

                var elementType = attacker.SpecController.ElementAttackType;
                if (elementType != Define.ElementType.None && !IsDead)
                {
                    var elementAttack = Mathf.FloorToInt(attacker.SpecController.ElementAttackTotal * attackSpec.ElementPower);
                    if (elementAttack > 0)
                    {
                        var projectileData = gameRules.ElementProjectiles.Get(elementType);
                        var elementProjectilePrefab = gameRules.ElementProjectiles.Get(elementType).ProjectilePrefab;
                        var elementProjectile = projectileData.ProjectilePrefab.Spawn(attacker, masterData.AttackSpecs.Get(projectileData.AttackSpecKey), impactPosition, Quaternion.identity);
                        elementProjectile.transform.SetParent(actor.transform);
                    }
                }

                if (fixedHitPoint <= 0)
                {
                    actor.StateMachine.TryChangeState(spec.DeadSequences, force: true);
                    onDead.OnNext(Unit.Default);
                    deadCancellationTokenSource.Cancel();
                    deadCancellationTokenSource.Dispose();
                }
                else if (CanPlayFlinch() || attackSpec.ForceFlinch)
                {
                    var lookAt = attacker.transform.position - actor.transform.position;
                    lookAt.y = 0.0f;
                    actor.MovementController.RotateImmediate(Quaternion.LookRotation(lookAt));
                    actor.MovementController.CanRotate.Value = false;
                    flinchType.Value = attackSpec.FlinchType;
                    actor.StateMachine.TryChangeState(FlinchStateSequences, force: true, containerAction: c => c.Register("FlinchName", attackSpec.FlinchName));
                    ResetFlinch();
                    onFlinch.OnNext(Unit.Default);
                }

                if (attacker.SpecController.ActorType == Define.ActorType.Player && attackSpec.HitAdditionalSequencesPlayer != null)
                {
                    var container = new Container();
                    var sequencer = new Sequencer(container, attackSpec.HitAdditionalSequencesPlayer.Sequences);
                    sequencer.PlayAsync(actor.destroyCancellationToken).Forget();
                }

                if (guardResult == Define.GuardResult.SuccessGuard)
                {
                    actor.StateMachine.TryChangeState(spec.SuccessGuardSequences, force: true);
                }
                onTakeDamage.OnNext(damageData);
            }
        }

        public void TakeDamageFromPoison(int damage)
        {
            var fixedHitPoint = hitPoint.Value - damage;
            fixedHitPoint = fixedHitPoint < 1 ? 1 : fixedHitPoint;
            hitPoint.Value = fixedHitPoint;
            onTakeDamage.OnNext(new DamageData(damage, 0, actor.LocatorHolder.Get("Spine").position + Random.insideUnitSphere));
        }

        public bool ContainsAppliedAbnormalStatus(Define.AbnormalStatusType type)
        {
            return appliedAbnormalStatuses.Contains(type);
        }

        public void RemoveAppliedAbnormalStatus(Define.AbnormalStatusType type)
        {
            appliedAbnormalStatuses.Remove(type);
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
            if (appliedAbnormalStatuses.Contains(Define.AbnormalStatusType.Paralysis))
            {
                return false;
            }
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
            hitPoint.Value = hitPointMax.Value;
            flinch.Value = 0;
            flinchType.Value = Define.FlinchType.None;
            CanAddFlinchDamage.Value = true;
            Invincible.Value = false;
            recoveryCommandCount.Value = spec.RecoveryCommandCount;
            actor.StateMachine.TryChangeState(spec.InitialStateSequences, force: true);
        }

        public bool TryRecovery()
        {
            if (hitPoint.Value >= spec.HitPoint || recoveryCommandCount.Value <= 0)
            {
                return false;
            }
            return actor.StateMachine.TryChangeState(spec.RecoverySequences);
        }

        public void AddRecoveryCommandCount(int value)
        {
            recoveryCommandCount.Value += value;
        }

#if DEBUG
        public void SetHitPointDebug(int value)
        {
            hitPoint.Value = value;
        }
#endif
    }
}
