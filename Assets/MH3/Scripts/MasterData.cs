using System;
using System.Collections.Generic;
using System.Linq;
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
        private WeaponCombo.Group weaponCombos;
        public WeaponCombo.Group WeaponCombos => weaponCombos;

        [SerializeField]
        private AttackSpec.DictionaryList attackSpecs;
        public AttackSpec.DictionaryList AttackSpecs => attackSpecs;

        [SerializeField]
        private ActorSpec.DictionaryList actorSpecs;
        public ActorSpec.DictionaryList ActorSpecs => actorSpecs;

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
            };
            var database = await UniTask.WhenAll(
                masterDataNames.Select(GoogleSpreadSheetDownloader.DownloadAsync)
            );
            weaponSpecs.Set(JsonHelper.FromJson<WeaponSpec>(database[0]));
            attackSpecs.Set(JsonHelper.FromJson<AttackSpec>(database[1]));
            actorSpecs.Set(JsonHelper.FromJson<ActorSpec>(database[2]));
            weaponCombos.Set(JsonHelper.FromJson<WeaponCombo>(database[3]));
            foreach (var weaponSpec in weaponSpecs.List)
            {
                weaponSpec.ModelData = AssetDatabase.LoadAssetAtPath<WeaponModelData>($"Assets/MH3/Database/WeaponModelData/{weaponSpec.Id}.asset");
            }
            foreach (var actorSpec in actorSpecs.List)
            {
                actorSpec.InitialStateSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.{actorSpec.InitialStateKey}.asset");
                actorSpec.AttackSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.{actorSpec.AttackSequencesKey}.asset");
                actorSpec.FlinchSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.{actorSpec.FlinchSequencesKey}.asset");
                actorSpec.DodgeSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.{actorSpec.DodgeSequencesKey}.asset");
                actorSpec.GuardSequences = AssetDatabase.LoadAssetAtPath<ScriptableSequences>($"Assets/MH3/Database/StateSequences/State.{actorSpec.GuardSequencesKey}.asset");
                actorSpec.ActorPrefab = AssetDatabase.LoadAssetAtPath<Actor>($"Assets/MH3/Prefabs/Actor.{actorSpec.ActorPrefabKey}.prefab");
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

            public int ComboId;

            public WeaponModelData ModelData;
            
            [Serializable]
            public class DictionaryList : DictionaryList<int, WeaponSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }

            public List<WeaponCombo> GetCombos()
            {
                return TinyServiceLocator.Resolve<MasterData>().WeaponCombos.Get(ComboId);
            }
        }

        [Serializable]
        public class WeaponCombo
        {
            public int Id;

            public string AnimationKey;

            [Serializable]
            public class Group : Group<int, WeaponCombo>
            {
                public Group() : base(x => x.Id)
                {
                }
            }
        }

        [Serializable]
        public class AttackSpec
        {
            public int Id;

            public int Power;

            public int FlinchDamage;

            public string ColliderName;

            public string FlinchName;

            public float HitStopTimeScaleActor;

            public float HitStopDurationActor;

            public float HitStopTimeScaleTarget;

            public float HitStopDurationTarget;

            [Serializable]
            public class DictionaryList : DictionaryList<int, AttackSpec>
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

            public int HitPoint;

            public int FlinchThreshold;

            public float MoveSpeed;

            public float RotationSpeed;

            public int WeaponId;

            public string InitialStateKey;

            public string AttackSequencesKey;

            public string FlinchSequencesKey;

            public string DodgeSequencesKey;

            public string GuardSequencesKey;

            public string ActorPrefabKey;

            public ScriptableSequences InitialStateSequences;

            public ScriptableSequences AttackSequences;

            public ScriptableSequences FlinchSequences;

            public ScriptableSequences DodgeSequences;

            public ScriptableSequences GuardSequences;

            public Actor ActorPrefab;

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
    }
}