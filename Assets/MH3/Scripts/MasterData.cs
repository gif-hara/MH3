using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEditor;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MasterData", menuName = "MH3/MasterData")]
    public sealed class MasterData : ScriptableObject
    {
        [SerializeField]
        private WeaponSpec.DictionaryList weaponSpecs;
        public WeaponSpec.DictionaryList WeaponSpecs => weaponSpecs;

        [SerializeField]
        private WeaponAttack.Group weaponAttacks;
        public WeaponAttack.Group WeaponAttacks => weaponAttacks;

        [SerializeField]
        private WeaponCritical.Group weaponCriticals;
        public WeaponCritical.Group WeaponCriticals => weaponCriticals;

        [SerializeField]
        private WeaponSkillSlot.Group weaponSkillSlots;
        public WeaponSkillSlot.Group WeaponSkillSlots => weaponSkillSlots;

        [SerializeField]
        private WeaponAbnormalStatus.Group weaponAbnormalStatuses;
        public WeaponAbnormalStatus.Group WeaponAbnormalStatuses => weaponAbnormalStatuses;

        [SerializeField]
        private WeaponElement.Group weaponElements;
        public WeaponElement.Group WeaponElements => weaponElements;

        [SerializeField]
        private WeaponCombo.Group weaponCombos;
        public WeaponCombo.Group WeaponCombos => weaponCombos;

        [SerializeField]
        private AttackSpec.DictionaryList attackSpecs;
        public AttackSpec.DictionaryList AttackSpecs => attackSpecs;

        [SerializeField]
        private ActorSpec.DictionaryList actorSpecs;
        public ActorSpec.DictionaryList ActorSpecs => actorSpecs;

        [SerializeField]
        private QuestSpec.DictionaryList questSpecs;
        public QuestSpec.DictionaryList QuestSpecs => questSpecs;

        [SerializeField]
        private QuestReward.Group questRewards;
        public QuestReward.Group QuestRewards => questRewards;

        [SerializeField]
        private SkillCoreSpec.DictionaryList skillCoreSpecs;
        public SkillCoreSpec.DictionaryList SkillCoreSpecs => skillCoreSpecs;

        [SerializeField]
        private SkillCoreCount.Group skillCoreCounts;
        public SkillCoreCount.Group SkillCoreCounts => skillCoreCounts;

        [SerializeField]
        private SkillCoreEffect.Group skillCoreEffects;
        public SkillCoreEffect.Group SkillCoreEffects => skillCoreEffects;

        [SerializeField]
        private SkillTypeToParameter.Group skillTypeToParameters;
        public SkillTypeToParameter.Group SkillTypeToParameters => skillTypeToParameters;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillAttackUp;
        public SkillLevelValue.DictionaryList SkillAttackUp => skillAttackUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillCriticalUp;
        public SkillLevelValue.DictionaryList SkillCriticalUp => skillCriticalUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillDefenseUp;
        public SkillLevelValue.DictionaryList SkillDefenseUp => skillDefenseUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillAbnormalStatusUp;
        public SkillLevelValue.DictionaryList SkillAbnormalStatusUp => skillAbnormalStatusUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillElementAttackUp;
        public SkillLevelValue.DictionaryList SkillElementAttackUp => skillElementAttackUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillHealthUp;
        public SkillLevelValue.DictionaryList SkillHealthUp => skillHealthUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillRecoveryCommandCountUp;
        public SkillLevelValue.DictionaryList SkillRecoveryCommandCountUp => skillRecoveryCommandCountUp;

        [SerializeField]
        private SkillLevelValue.DictionaryList skillRewardUp;
        public SkillLevelValue.DictionaryList SkillRewardUp => skillRewardUp;

#if UNITY_EDITOR
        [ContextMenu("Update")]
        private async void UpdateMasterData()
        {
            var startTime = DateTime.Now;
            var progressId = UnityEditor.Progress.Start("MasterData Update");
            var scope = new CancellationTokenSource();
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    var elapsed = DateTime.Now - startTime;
                    UnityEditor.Progress.Report(progressId, (float)elapsed.TotalSeconds / 10.0f);
                })
                .RegisterTo(scope.Token);
            Debug.Log("Begin MasterData Update");
            var masterDataNames = new[]
            {
                "WeaponSpec",
                "AttackSpec",
                "ActorSpec",
                "WeaponCombo",
                "QuestSpec",
                "WeaponAttack",
                "QuestReward",
                "WeaponCritical",
                "SkillCoreSpec",
                "SkillCoreCount",
                "SkillCoreEffect",
                "WeaponSkillSlot",
                "Skill.AttackUp",
                "SkillTypeToParameter",
                "Skill.CriticalUp",
                "Skill.DefenseUp",
                "WeaponAbnormalStatus",
                "WeaponElement",
                "Skill.AbnormalStatusUp",
                "Skill.ElementAttackUp",
                "Skill.HealthUp",
                "Skill.RecoveryCommandCountUp",
                "Skill.RewardUp",
            };
            var database = await UniTask.WhenAll(
                masterDataNames.Select(GoogleSpreadSheetDownloader.DownloadAsync)
            );
            weaponSpecs.Set(JsonHelper.FromJson<WeaponSpec>(database[0]));
            attackSpecs.Set(JsonHelper.FromJson<AttackSpec>(database[1]));
            actorSpecs.Set(JsonHelper.FromJson<ActorSpec>(database[2]));
            weaponCombos.Set(JsonHelper.FromJson<WeaponCombo>(database[3]));
            questSpecs.Set(JsonHelper.FromJson<QuestSpec>(database[4]));
            weaponAttacks.Set(JsonHelper.FromJson<WeaponAttack>(database[5]));
            questRewards.Set(JsonHelper.FromJson<QuestReward>(database[6]));
            weaponCriticals.Set(JsonHelper.FromJson<WeaponCritical>(database[7]));
            skillCoreSpecs.Set(JsonHelper.FromJson<SkillCoreSpec>(database[8]));
            skillCoreCounts.Set(JsonHelper.FromJson<SkillCoreCount>(database[9]));
            skillCoreEffects.Set(JsonHelper.FromJson<SkillCoreEffect>(database[10]));
            weaponSkillSlots.Set(JsonHelper.FromJson<WeaponSkillSlot>(database[11]));
            skillAttackUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[12]));
            skillTypeToParameters.Set(JsonHelper.FromJson<SkillTypeToParameter>(database[13]));
            skillCriticalUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[14]));
            skillDefenseUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[15]));
            weaponAbnormalStatuses.Set(JsonHelper.FromJson<WeaponAbnormalStatus>(database[16]));
            weaponElements.Set(JsonHelper.FromJson<WeaponElement>(database[17]));
            skillAbnormalStatusUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[18]));
            skillElementAttackUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[19]));
            skillHealthUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[20]));
            skillRecoveryCommandCountUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[21]));
            skillRewardUp.Set(JsonHelper.FromJson<SkillLevelValue>(database[22]));
            foreach (var weaponSpec in weaponSpecs.List)
            {
                weaponSpec.ModelData = AssetDatabase.LoadAssetAtPath<WeaponModelData>($"Assets/MH3/Database/WeaponModelData/{weaponSpec.ModelDataId}.asset");
            }
            foreach (var actorSpec in actorSpecs.List)
            {
                actorSpec.InitialStateSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.InitialStateKey}.asset");
                actorSpec.AttackSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.AttackSequencesKey}.asset");
                actorSpec.FlinchSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.FlinchSequencesKey}.asset");
                actorSpec.DodgeSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.DodgeSequencesKey}.asset");
                actorSpec.GuardSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.GuardSequencesKey}.asset");
                actorSpec.SuccessJustGuardSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.SuccessJustGuardSequencesKey}.asset");
                actorSpec.SuccessGuardSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.SuccessGuardSequencesKey}.asset");
                actorSpec.DeadSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.DeadSequencesKey}.asset");
                actorSpec.RecoverySequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.Enter.{actorSpec.RecoverySequencesKey}.asset");
                actorSpec.ActorPrefab = AssetDatabase.LoadAssetAtPath<Actor>($"Assets/MH3/Prefabs/Actor.{actorSpec.ActorPrefabKey}.prefab");
                actorSpec.Behaviour = AssetDatabase.LoadAssetAtPath<ActorBehaviourData>($"Assets/MH3/Database/ActorBehaviours/{actorSpec.BehaviourKey}.asset");
            }
            foreach (var questSpec in questSpecs.List)
            {
                questSpec.StagePrefab = AssetDatabase.LoadAssetAtPath<Stage>($"Assets/MH3/Prefabs/Stage.{questSpec.StagePrefabKey}.prefab");
                questSpec.QuestClearSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/QuestClearSequences/{questSpec.QuestClearSequencesKey}.asset");
                questSpec.QuestFailedSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/QuestFailedSequences/{questSpec.QuestFailedSequencesKey}.asset");
            }
            foreach (var attackSpec in attackSpecs.List)
            {
                attackSpec.HitAdditionalSequencesPlayer = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/HitAdditionalSequences/{attackSpec.HitAdditionalSequencesKeyPlayer}.asset");
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            Debug.Log("End MasterData Update");
            UnityEditor.Progress.Remove(progressId);
            scope.Cancel();
            scope.Dispose();
        }
#endif

        [Serializable]
        public class WeaponSpec
        {
            public int Id;

            public string Name;

            public string ModelDataId;

            public string ComboId;

            public string JustGuardAttackAnimationKey;

            public string StrongAttackAnimationKey;

            public WeaponModelData ModelData;

            [Serializable]
            public class DictionaryList : DictionaryList<int, WeaponSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }

            public List<WeaponAttack> GetAttacks()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponAttacks.Get(Id);
            }

            public List<WeaponCritical> GetCriticals()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponCriticals.Get(Id);
            }

            public List<WeaponCombo> GetCombos()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponCombos.Get(ComboId);
            }

            public List<WeaponSkillSlot> GetSkillSlots()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponSkillSlots.Get(Id);
            }

            public List<WeaponAbnormalStatus> GetAbnormalStatuses()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponAbnormalStatuses.Get(Id);
            }

            public List<WeaponElement> GetElements()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponElements.Get(Id);
            }
        }

        [Serializable]
        public class WeaponAttack
        {
            public int Id;

            public int Attack;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, WeaponAttack>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class WeaponCritical
        {
            public int Id;

            public float Critical;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, WeaponCritical>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class WeaponSkillSlot
        {
            public int Id;

            public int SkillSlot;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, WeaponSkillSlot>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class WeaponAbnormalStatus
        {
            public int Id;

            public Define.AbnormalStatusType AbnormalStatusType;

            public int Power;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, WeaponAbnormalStatus>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class WeaponElement
        {
            public int Id;

            public Define.ElementType ElementType;

            public int Power;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, WeaponElement>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class WeaponCombo
        {
            public string Id;

            public string AnimationKey;

            [Serializable]
            public class Group : Group<string, WeaponCombo>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class AttackSpec
        {
            public string Id;

            public int Power;

            public int FlinchDamage;

            public string ColliderName;

            public string FlinchName;

            public Define.FlinchType FlinchType;

            public bool ForceFlinch;

            public float AbnormalStatusPower;

            public float ElementPower;

            public Define.ElementType ElementType;

            public bool CanCritical;

            public float HitStopTimeScaleActor;

            public float HitStopDurationActor;

            public float HitStopTimeScaleTarget;

            public float HitStopDurationTarget;

            public float ShakeStrength;

            public float ShakeDuration;

            public int ShakeFrequency;

            public float ShakeDampingRatio;

            public string HitSfxKey;

            public string HitEffectKey;

            public string HitCameraImpulseSourceKey;

            public string HitAdditionalSequencesKeyPlayer;

            public ScriptableSequences HitAdditionalSequencesPlayer;

            [Serializable]
            public class DictionaryList : DictionaryList<string, AttackSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class ActorSpec
        {
            public int Id;

            public Define.ActorType ActorType;

            public int HitPoint;

            public int Attack;

            public float PhysicalDamageCutRate;

            public float FireDamageCutRate;

            public float WaterDamageCutRate;

            public float GrassDamageCutRate;

            public int FlinchThreshold;

            public int PoisonThreshold;

            public int ParalysisThreshold;

            public int CollapseThreshold;

            public int PoisonDuration;

            public int ParalysisDuration;

            public int CollapseDuration;

            public float MoveSpeed;

            public float RotationSpeed;

            public int WeaponId;

            public bool VisibleStatusUI;

            public int RecoveryCommandCount;

            public string InitialStateKey;

            public string AttackSequencesKey;

            public string FlinchSequencesKey;

            public string DodgeSequencesKey;

            public string GuardSequencesKey;

            public string SuccessJustGuardSequencesKey;

            public string SuccessGuardSequencesKey;

            public string DeadSequencesKey;

            public string RecoverySequencesKey;

            public string ActorPrefabKey;

            public string BehaviourKey;

            public ScriptableSequences InitialStateSequences;

            public ScriptableSequences AttackSequences;

            public ScriptableSequences FlinchSequences;

            public ScriptableSequences DodgeSequences;

            public ScriptableSequences GuardSequences;

            public ScriptableSequences SuccessJustGuardSequences;

            public ScriptableSequences SuccessGuardSequences;

            public ScriptableSequences DeadSequences;

            public ScriptableSequences RecoverySequences;

            public Actor ActorPrefab;

            public ActorBehaviourData Behaviour;

            [Serializable]
            public class DictionaryList : DictionaryList<int, ActorSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }

            public Actor Spawn(Vector3 position, Quaternion rotation)
            {
                return ActorPrefab.Spawn(position, rotation, this);
            }
        }

        [Serializable]
        public class QuestSpec
        {
            public string Id;

            public string StagePrefabKey;

            public int EnemyActorSpecId;

            public string QuestClearSequencesKey;

            public string QuestFailedSequencesKey;

            public bool AvailableQuestList;

            public int RewardCount;

            public Stage StagePrefab;

            public ScriptableSequences QuestClearSequences;

            public ScriptableSequences QuestFailedSequences;

            public List<QuestReward> GetRewards()
            {
                return TinyServiceLocator.Resolve<MasterData>().QuestRewards.Get(Id);
            }

            [Serializable]
            public class DictionaryList : DictionaryList<string, QuestSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class QuestReward
        {
            public string Id;

            public Define.RewardType RewardType;

            public int RewardId;

            public int Weight;

            [Serializable]
            public class Group : Group<string, QuestReward>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class SkillCoreSpec
        {
            public int Id;

            public string Name;

            public int Slot;

            public string ColorCode;

            public Color Color => ColorUtility.TryParseHtmlString(ColorCode, out var color) ? color : Color.white;

            public List<SkillCoreCount> GetSkillCoreCounts()
            {
                return TinyServiceLocator.Resolve<MasterData>().SkillCoreCounts.Get(Id);
            }

            public List<SkillCoreEffect> GetSkillCoreEffects()
            {
                return TinyServiceLocator.Resolve<MasterData>().SkillCoreEffects.Get(Id);
            }

            [Serializable]
            public class DictionaryList : DictionaryList<int, SkillCoreSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class SkillCoreCount
        {
            public int Id;

            public int Count;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, SkillCoreCount>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class SkillCoreEffect
        {
            public int Id;

            public Define.SkillType SkillType;

            public int Level;

            public Define.RareType RareType;

            public int Weight;

            [Serializable]
            public class Group : Group<int, SkillCoreEffect>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class SkillLevelValue
        {
            public int Level;

            public float Value;

            [Serializable]
            public class DictionaryList : DictionaryList<int, SkillLevelValue>
            {
                public DictionaryList() : base(x => x.Level)
                {
                }
            }
        }

        [Serializable]
        public class SkillTypeToParameter
        {
            public Define.SkillType SkillType;

            public Define.ActorParameterType ActorParameterType;

            public Define.SkillLevelValueType SkillLevelValueType;

            [Serializable]
            public class Group : Group<Define.SkillType, SkillTypeToParameter>
            {
                public Group() : base(x => x.SkillType)
                {
                }
            }
        }
    }
}