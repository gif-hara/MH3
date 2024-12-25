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
    public class QuestSpecViewClearTime : Sequence
    {
        [SerializeField]
        private string questSpecKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpec = container.Resolve<MasterData.QuestSpec>(questSpecKey);
            var userData = TinyServiceLocator.Resolve<UserData>();
            var questClearTimeKey = Stats.Key.GetQuestClearTime(questSpec.Id);
            var elapsedQuestTime = userData.Stats.GetOrDefault(questClearTimeKey);
            var timeSpan = TimeSpan.FromSeconds(elapsedQuestTime);
            text.text = elapsedQuestTime <= 0.0f
            ? "--:--:--"
            : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D3}";
            return UniTask.CompletedTask;
        }
    }
}
