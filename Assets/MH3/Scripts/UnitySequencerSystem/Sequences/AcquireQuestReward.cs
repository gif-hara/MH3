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
    public class AcquireQuestReward : Sequence
    {
        [SerializeField]
        private HKUIDocument documentPrefab;

        [SerializeReference, SubclassSelector]
        private ActorResolver playerActorResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver enemyActorResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver questSpecIdResolver;

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpecId = questSpecIdResolver.Resolve(container);
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            var userData = TinyServiceLocator.Resolve<UserData>();
            var player = playerActorResolver.Resolve(container);
            var enemy = enemyActorResolver.Resolve(container);
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var gameSceneController = container.Resolve<GameSceneController>();

            for (var i = 0; i < questSpec.RewardCount + player.SpecController.RewardUp.CurrentValue; i++)
            {
                var rewards = new List<IReward>();
                for (var k = 0; k < gameRules.RewardOptionNumber; k++)
                {
                    var reward = questSpec.GetRewards().Lottery(x => x.Weight);
                    switch (reward.RewardType)
                    {
                        case Define.RewardType.InstanceWeapon:
                            rewards.Add(InstanceWeaponFactory.Create(userData, reward.RewardId));
                            break;
                        case Define.RewardType.InstanceSkillCore:
                            rewards.Add(InstanceSkillCoreFactory.Create(userData, reward.RewardId));
                            break;
                        case Define.RewardType.InstanceArmor:
                            rewards.Add(InstanceArmorFactory.Create(userData, reward.RewardId));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"未対応のRewardTypeです {reward.RewardType}");
                    }
                }
                var index = await UIViewAcquireReward.OpenAsync(documentPrefab, rewards, gameSceneController.ElapsedQuestTime, enemy.SpecController.Name, cancellationToken);
                rewards[index].Acquire(userData);
            }
        }
    }
}
