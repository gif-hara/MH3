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
    public class QuestSpecViewDefeatEnemyCount : Sequence
    {
        [SerializeField]
        private string questSpecKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpec = container.Resolve<MasterData.QuestSpec>(questSpecKey);
            var userData = TinyServiceLocator.Resolve<UserData>();
            var defeatEnemyCount = userData.Stats.GetOrDefault(Stats.Key.GetDefeatEnemyCount(questSpec.EnemyActorSpecId));
            text.text = defeatEnemyCount.ToString();
            return UniTask.CompletedTask;
        }
    }
}
