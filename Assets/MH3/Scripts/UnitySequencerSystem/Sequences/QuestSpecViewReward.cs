using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class QuestSpecViewReward : Sequence
    {
        [SerializeField]
        private string questSpecKey;

        [SerializeField]
        private Transform parent;

        [SerializeField]
        private HKUIDocument labelDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpec = container.Resolve<MasterData.QuestSpec>(questSpecKey);
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
            foreach (var reward in questSpec.GetRewards())
            {
                var document = UnityEngine.Object.Instantiate(labelDocumentPrefab, parent);
                document.Q<TMP_Text>("Label").text = reward.GetName();
            }
            return UniTask.CompletedTask;
        }
    }
}
