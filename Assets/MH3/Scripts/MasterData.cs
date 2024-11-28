using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEditor;
using UnityEngine;

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
            };
            var database = await UniTask.WhenAll(
                masterDataNames.Select(GoogleSpreadSheetDownloader.DownloadAsync)
            );
            weaponSpecs.Set(JsonHelper.FromJson<WeaponSpec>(database[0]));
            foreach (var weaponSpec in weaponSpecs.List)
            {
                weaponSpec.ModelData = AssetDatabase.LoadAssetAtPath<WeaponModelData>($"Assets/MH3/Database/WeaponModelData/{weaponSpec.Id}.asset");
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

            public WeaponModelData ModelData;

            [Serializable]
            public class DictionaryList : DictionaryList<int, WeaponSpec>
            {
                public DictionaryList() : base(x => x.Id)
                {
                }
            }
        }
    }
}