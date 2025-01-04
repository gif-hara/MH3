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

        public string ActorName { get; set; }

        public readonly Parameter HitPointMax = new();

        private readonly ReactiveProperty<int> hitPoint = new(0);

        public readonly Parameter Attack = new();

        public readonly Parameter Defense = new();

        public readonly Parameter Critical = new();

        public readonly Parameter CutRatePhysicalDamage = new();

        public readonly Parameter CutRateFireDamage = new();

        public readonly Parameter CutRateWaterDamage = new();

        public readonly Parameter CutRateGrassDamage = new();

        public readonly Parameter AbnormalStatusAttack = new();

        private readonly ReactiveProperty<Define.AbnormalStatusType> abnormalStatusAttackType = new(Define.AbnormalStatusType.None);

        public readonly Parameter ElementAttack = new();

        private readonly ReactiveProperty<Define.ElementType> elementAttackType = new(Define.ElementType.None);

        private readonly Dictionary<Define.AbnormalStatusType, ReactiveProperty<int>> abnormalStatusThreshold = new();

        private readonly Dictionary<Define.AbnormalStatusType, ReactiveProperty<int>> abnormalStatusValues = new();

        private readonly HashSet<Define.AbnormalStatusType> appliedAbnormalStatuses = new();

        public readonly Parameter RecoveryCommandCountMax = new();

        private readonly ReactiveProperty<int> recoveryCommandCount = new();

        public readonly Parameter RewardUp = new();

        private readonly ReactiveProperty<int> weaponId = new(0);

        private readonly ReactiveProperty<int> armorHeadId = new(0);

        private readonly ReactiveProperty<int> armorArmsId = new(0);

        private readonly ReactiveProperty<int> armorBodyId = new(0);

        private readonly ReactiveProperty<int> flinch = new(0);

        public readonly ReactiveProperty<bool> CanAddFlinchDamage = new(true);

        public readonly ReactiveProperty<bool> Invincible = new(false);

        public readonly List<string> ComboAnimationKeys = new();

        public string JustGuardAttackAnimationKey { get; private set; }

        public string StrongAttackAnimationKey { get; private set; }

        public readonly ReactiveProperty<Actor> Target = new(null);

        private readonly ReactiveProperty<Define.FlinchType> flinchType = new(Define.FlinchType.None);

        public readonly List<ISkill> Skills = new();

        private readonly Subject<Unit> onFlinch = new();

        private readonly Subject<DamageData> onTakeDamage = new();

        private readonly Subject<Unit> onDead = new();

        private readonly CancellationTokenSource deadCancellationTokenSource = new();

        public ScriptableSequences GuardPerformedSequences { get; private set; }

        public ScriptableSequences GuardCanceledSequences { get; private set; }

        public ScriptableSequences DodgePerformedSequences { get; private set; }

        private readonly ReactiveProperty<int> superArmorCount = new(0);
        public ReadOnlyReactiveProperty<int> SuperArmorCount => superArmorCount;

        public readonly ReactiveProperty<bool> IsEventStop = new(false);

        private readonly Subject<Unit> onBuildStatuses = new();
        public Observable<Unit> OnBuildStatuses => onBuildStatuses;

        public ActorSpecController(Actor actor, MasterData.ActorSpec spec)
        {
            this.actor = actor;
            this.spec = spec;
            ActorName = spec.LocalizedName;
            HitPointMax.RegisterBasics("Spec", () => spec.HitPoint);
            hitPoint.Value = spec.HitPoint;
            Attack.RegisterBasics("Spec", () => spec.Attack);
            CutRatePhysicalDamage.RegisterBasics("Spec", () => spec.PhysicalDamageCutRate);
            CutRateFireDamage.RegisterBasics("Spec", () => spec.FireDamageCutRate);
            CutRateWaterDamage.RegisterBasics("Spec", () => spec.WaterDamageCutRate);
            CutRateGrassDamage.RegisterBasics("Spec", () => spec.GrassDamageCutRate);
            RecoveryCommandCountMax.RegisterBasics("Spec", () => spec.RecoveryCommandCount);
            recoveryCommandCount.Value = RecoveryCommandCountMax.ValueFloorToInt;
            SetWeaponId(spec.WeaponId);
            SetArmorId(Define.ArmorType.Head, spec.ArmorHeadId);
            SetArmorId(Define.ArmorType.Arms, spec.ArmorArmsId);
            SetArmorId(Define.ArmorType.Body, spec.ArmorBodyId);
            abnormalStatusThreshold.Add(Define.AbnormalStatusType.Poison, new ReactiveProperty<int>(spec.PoisonThreshold));
            abnormalStatusThreshold.Add(Define.AbnormalStatusType.Paralysis, new ReactiveProperty<int>(spec.ParalysisThreshold));
            abnormalStatusThreshold.Add(Define.AbnormalStatusType.Collapse, new ReactiveProperty<int>(spec.CollapseThreshold));
        }

        public Define.ActorType ActorType => spec.ActorType;

        public int HitPointBase => spec.HitPoint;

        public int HitPointMaxTotal => HitPointMax.ValueFloorToInt;

        public ReadOnlyReactiveProperty<int> HitPoint => hitPoint;

        public int AttackTotal => Attack.ValueFloorToInt;

        public float CriticalTotal => Critical.Value;

        public int AbnormalStatusAttackTotal => AbnormalStatusAttack.ValueFloorToInt;

        public Define.AbnormalStatusType AbnormalStatusAttackType => abnormalStatusAttackType.Value;

        public int ElementAttackTotal => ElementAttack.ValueFloorToInt;

        public Define.ElementType ElementAttackType => elementAttackType.Value;

        public int DefenseTotal => Defense.ValueFloorToInt;

        public ReadOnlyReactiveProperty<int> RecoveryCommandCount => recoveryCommandCount;

        public int PoisonDuration => spec.PoisonDuration;

        public int ParalysisDuration => spec.ParalysisDuration;

        public int CollapseDuration => spec.CollapseDuration;

        public float MoveSpeed => spec.MoveSpeed;

        public float RotationSpeed => spec.RotationSpeed;

        public bool VisibleStatusUI => spec.VisibleStatusUI;

        public bool IsDead => hitPoint.Value <= 0;

        public ReadOnlyReactiveProperty<int> WeaponId => weaponId;

        public ReadOnlyReactiveProperty<int> ArmorHeadId => armorHeadId;

        public ReadOnlyReactiveProperty<int> ArmorArmsId => armorArmsId;

        public ReadOnlyReactiveProperty<int> ArmorBodyId => armorBodyId;

        public ReadOnlyReactiveProperty<int> Flinch => flinch;

        public ScriptableSequences AttackStateSequences => spec.AttackSequences;

        public ScriptableSequences FlinchStateSequences => spec.FlinchSequences;

        public ScriptableSequences DodgeStateSequences => spec.DodgeSequences;

        public ScriptableSequences GuardStateSequences => spec.GuardSequences;

        public ScriptableSequences SuccessJustGuardSequences => spec.SuccessJustGuardSequences;

        public ScriptableSequences SuccessGuardSequences => spec.SuccessGuardSequences;

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

        public float GetCutRate(Define.ElementType elementType)
        {
            return elementType switch
            {
                Define.ElementType.None => CutRatePhysicalDamage.Value,
                Define.ElementType.Fire => CutRateFireDamage.Value,
                Define.ElementType.Water => CutRateWaterDamage.Value,
                Define.ElementType.Grass => CutRateGrassDamage.Value,
                _ => throw new System.NotImplementedException($"未対応の属性です {elementType}"),
            };
        }

        private void SetWeaponId(int value)
        {
            weaponId.Value = value;
            var weaponSpec = WeaponSpec;
            GuardPerformedSequences = weaponSpec.GuardPerformedSequences;
            GuardCanceledSequences = weaponSpec.GuardCanceledSequences;
            DodgePerformedSequences = weaponSpec.DodgePerformedSequences;
            JustGuardAttackAnimationKey = weaponSpec.JustGuardAttackAnimationKey;
            StrongAttackAnimationKey = weaponSpec.StrongAttackAnimationKey;
            ComboAnimationKeys.Clear();
            foreach (var combo in weaponSpec.GetCombos())
            {
                ComboAnimationKeys.Add(combo.AnimationKey);
            }
        }

        public void ChangeInstanceWeapon(InstanceWeapon instanceWeapon)
        {
            SetWeaponId(instanceWeapon.WeaponId);
            BuildStatuses();
        }

        public void SetArmorId(Define.ArmorType armorType, int armorId)
        {
            switch (armorType)
            {
                case Define.ArmorType.Head:
                    armorHeadId.Value = armorId;
                    break;
                case Define.ArmorType.Arms:
                    armorArmsId.Value = armorId;
                    break;
                case Define.ArmorType.Body:
                    armorBodyId.Value = armorId;
                    break;
                default:
                    throw new System.NotImplementedException($"未対応のArmorTypeです {armorType}");
            }
        }

        public void BuildStatuses()
        {
            var userData = TinyServiceLocator.Resolve<UserData>();
            var instanceWeapon = userData.GetEquippedInstanceWeapon();
            var instanceArmorHead = userData.GetEquippedInstanceArmorHead();
            var instanceArmorArms = userData.GetEquippedInstanceArmorArms();
            var instanceArmorBody = userData.GetEquippedInstanceArmorBody();
            var instanceWeaponSkills = instanceWeapon.InstanceSkillCores
                .SelectMany(x => x.Skills)
                .ToList();
            var instanceArmorHeadSkills = instanceArmorHead?.Skills ?? new List<InstanceSkill>();
            var instanceArmorArmsSkills = instanceArmorArms?.Skills ?? new List<InstanceSkill>();
            var instanceArmorBodySkills = instanceArmorBody?.Skills ?? new List<InstanceSkill>();
            var instanceSkills = instanceWeaponSkills
                .Concat(instanceArmorHeadSkills)
                .Concat(instanceArmorArmsSkills)
                .Concat(instanceArmorBodySkills)
                .ToList();
            Attack.ClearAll();
            Critical.ClearAll();
            CutRatePhysicalDamage.ClearAll();
            CutRateFireDamage.ClearAll();
            CutRateWaterDamage.ClearAll();
            CutRateGrassDamage.ClearAll();
            AbnormalStatusAttack.ClearAll();
            ElementAttack.ClearAll();
            HitPointMax.ClearAll();
            Defense.ClearAll();
            RecoveryCommandCountMax.ClearAll();
            RewardUp.ClearAll();
            Critical.RegisterBasics("InstanceWeapon", () => instanceWeapon.Critical);
            CutRatePhysicalDamage.RegisterBasics("Spec", () => spec.PhysicalDamageCutRate);
            CutRateFireDamage.RegisterBasics("Spec", () => spec.FireDamageCutRate);
            CutRateWaterDamage.RegisterBasics("Spec", () => spec.WaterDamageCutRate);
            CutRateGrassDamage.RegisterBasics("Spec", () => spec.GrassDamageCutRate);
            AbnormalStatusAttack.RegisterBasics("InstanceWeapon", () => instanceWeapon.AbnormalStatusAttack);
            abnormalStatusAttackType.Value = instanceWeapon.AbnormalStatusType;
            ElementAttack.RegisterBasics("InstanceWeapon", () => instanceWeapon.ElementAttack);
            elementAttackType.Value = instanceWeapon.ElementType;
            HitPointMax.RegisterBasics("Spec", () => spec.HitPoint);
            hitPoint.Value = HitPointMaxTotal;
            Attack.RegisterBasics("Spec", () => spec.Attack);
            Attack.RegisterBasics("InstanceWeapon", () => instanceWeapon.Attack);
            Defense.RegisterBasics("InstanceArmorHead", () => instanceArmorHead?.Defense ?? 0);
            Defense.RegisterBasics("InstanceArmorArms", () => instanceArmorArms?.Defense ?? 0);
            Defense.RegisterBasics("InstanceArmorBody", () => instanceArmorBody?.Defense ?? 0);
            RecoveryCommandCountMax.RegisterBasics("Spec", () => spec.RecoveryCommandCount);
            recoveryCommandCount.Value = RecoveryCommandCountMax.ValueFloorToInt;
            Skills.Clear();
            Skills.AddRange(SkillFactory.CreateSkills(instanceSkills));
            foreach (var skill in Skills)
            {
                skill.Attach(actor);
            }
            onBuildStatuses.OnNext(Unit.Default);
        }

        public void SetSuperArmor(int value)
        {
            superArmorCount.Value = value;
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
            var audioManager = TinyServiceLocator.Resolve<AudioManager>();

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
                    audioManager.PlaySfx(TinyServiceLocator.Resolve<GameRules>().SuccessGuardSfxKey);
                }
                else
                {
                    audioManager.PlaySfx(attackSpec.HitSfxKey);
                }
                var hitEffect = TinyServiceLocator.Resolve<EffectManager>().Rent(attackSpec.HitEffectKey);
                hitEffect.transform.position = impactPosition;
                if (damageData.IsCritical)
                {
                    var criticalEffect = TinyServiceLocator.Resolve<EffectManager>().Rent(gameRules.CriticalHitEffectKey);
                    criticalEffect.transform.position = impactPosition;
                    audioManager.PlaySfx(gameRules.CriticalHitSfxKey);
                }

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

                if (superArmorCount.Value > 0)
                {
                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx(gameRules.SuperArmorHitSfxKey);
                }
                superArmorCount.Value--;

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
            if (IsDead || Target.Value.SpecController.IsDead)
            {
                return;
            }
            var fixedHitPoint = hitPoint.Value - damage;
            fixedHitPoint = fixedHitPoint < 1 ? 1 : fixedHitPoint;
            hitPoint.Value = fixedHitPoint;
            onTakeDamage.OnNext(new DamageData(damage, 0, actor.LocatorHolder.Get("Spine").position + Random.insideUnitSphere, false));
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
            if (superArmorCount.Value > 0)
            {
                return false;
            }
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
            hitPoint.Value = result > HitPointMaxTotal ? HitPointMaxTotal : result;
        }

        public void ResetAll()
        {
            hitPoint.Value = HitPointMaxTotal;
            flinch.Value = 0;
            flinchType.Value = Define.FlinchType.None;
            CanAddFlinchDamage.Value = true;
            Invincible.Value = false;
            appliedAbnormalStatuses.Clear();
            recoveryCommandCount.Value = RecoveryCommandCountMax.ValueFloorToInt;
            actor.StateMachine.TryChangeState(spec.InitialStateSequences, force: true);
        }

        public bool TryRecovery()
        {
            if (hitPoint.Value >= HitPointMaxTotal || recoveryCommandCount.Value <= 0)
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
