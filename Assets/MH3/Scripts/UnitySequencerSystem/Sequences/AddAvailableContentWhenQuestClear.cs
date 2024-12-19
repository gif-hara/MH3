using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class AddAvailableContentWhenQuestClear : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver questSpecIdResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var questSpecId = questSpecIdResolver.Resolve(container);
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            var userData = TinyServiceLocator.Resolve<UserData>();
            userData.AvailableContents.Add(questSpec.AddWhenQuestClearContentKey);
            return UniTask.CompletedTask;
        }
    }
}
