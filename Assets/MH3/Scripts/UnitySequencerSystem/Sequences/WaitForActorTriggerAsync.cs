using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.ActorControllers;
using MH3.UnitySequencerSystem.Resolvers;
using R3;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class WaitForActorTriggerAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ActorStateProvider.TriggerType triggerType;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            return actor.StateProvider.GetTriggerAsObservable(triggerType).FirstAsync().AsUniTask();
        }
    }
}
