using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.ActorControllers;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class ActorSetIntegerState : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ActorStateProvider.IntegerType type;

        [SerializeReference, SubclassSelector]
        private IntResolver valueResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var value = valueResolver.Resolve(container);
            actor.StateProvider.SetInteger(type, value);
            return UniTask.CompletedTask;
        }
    }
}
