using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class QuestSpecViewEnemyName : Sequence
    {
        [SerializeField]
        private string questSpecKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpec = container.Resolve<MasterData.QuestSpec>(questSpecKey);
            var actorSpec = questSpec.GetEnemyActorSpec();
            text.text = actorSpec.LocalizedName;
            return UniTask.CompletedTask;
        }
    }
}
