using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class AcquireQuestReward : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver questSpecIdResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpecId = questSpecIdResolver.Resolve(container);
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            var userData = TinyServiceLocator.Resolve<UserData>();
            var instanceWeapons = new List<InstanceWeaponData>();
            for (var i = 0; i < questSpec.RewardCount; i++)
            {
                var reward = questSpec.GetRewards().Lottery(x => x.Weight);
                switch (reward.RewardType)
                {
                    case Define.RewardType.InstanceWeapon:
                        var instanceWeaponData = InstanceWeaponFactory.Create(TinyServiceLocator.Resolve<UserData>(), reward.RewardId);
                        instanceWeapons.Add(instanceWeaponData);
                        userData.AddInstanceWeaponData(instanceWeaponData);
                        break;
                }
            }
            container.Register("AcquireInstanceWeapons", instanceWeapons);
            return UniTask.CompletedTask;
        }
    }
}
