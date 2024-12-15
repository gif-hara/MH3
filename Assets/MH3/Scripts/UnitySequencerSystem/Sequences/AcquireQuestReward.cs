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
        [SerializeReference, SubclassSelector]
        private ActorResolver playerActorResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver questSpecIdResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpecId = questSpecIdResolver.Resolve(container);
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            var userData = TinyServiceLocator.Resolve<UserData>();
            var instanceWeapons = new List<InstanceWeapon>();
            var instanceSkillCores = new List<InstanceSkillCore>();
            var player = playerActorResolver.Resolve(container);
            for (var i = 0; i < questSpec.RewardCount + player.SpecController.RewardUp.CurrentValue; i++)
            {
                var reward = questSpec.GetRewards().Lottery(x => x.Weight);
                switch (reward.RewardType)
                {
                    case Define.RewardType.InstanceWeapon:
                        var instanceWeaponData = InstanceWeaponFactory.Create(userData, reward.RewardId);
                        instanceWeapons.Add(instanceWeaponData);
                        userData.AddInstanceWeaponData(instanceWeaponData);
                        break;
                    case Define.RewardType.InstanceSkillCore:
                        var instanceSkillCore = InstanceSkillCoreFactory.Create(userData, reward.RewardId);
                        instanceSkillCores.Add(instanceSkillCore);
                        userData.AddInstanceSkillCoreData(instanceSkillCore);
                        break;
                }
            }
            container.Register("AcquireInstanceWeapons", instanceWeapons);
            container.Register("AcquireInstanceSkillCores", instanceSkillCores);
            return UniTask.CompletedTask;
        }
    }
}
