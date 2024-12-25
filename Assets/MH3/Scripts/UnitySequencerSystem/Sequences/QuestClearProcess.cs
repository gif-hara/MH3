using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class QuestClearProcess : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver questSpecIdResolver;

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpecId = questSpecIdResolver.Resolve(container);
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            var userData = TinyServiceLocator.Resolve<UserData>();
            var gameSceneController = container.Resolve<GameSceneController>();
            var questClearTimeKey = Stats.Key.GetQuestClearTime(questSpecId);
            var elapsedQuestTime = userData.Stats.GetOrDefault(questClearTimeKey);
            if (gameSceneController.ElapsedQuestTime < elapsedQuestTime || !userData.Stats.Contains(questClearTimeKey))
            {
                userData.Stats.AddOrUpdate(questClearTimeKey, gameSceneController.ElapsedQuestTime);
            }
            var defeatEnemyCount = userData.Stats.GetOrDefault(Stats.Key.GetDefeatEnemyCount(questSpec.EnemyActorSpecId));
            userData.Stats.AddOrUpdate(Stats.Key.GetDefeatEnemyCount(questSpec.EnemyActorSpecId), defeatEnemyCount + 1);
        }
    }
}
