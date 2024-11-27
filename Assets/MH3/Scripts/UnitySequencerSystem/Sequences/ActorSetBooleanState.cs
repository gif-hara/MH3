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
    public class ActorSetBooleanState : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ActorStateProvider.BooleanType type;
        
        [SerializeReference, SubclassSelector]
        private BooleanResolver isTrueResolver;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var isTrue = isTrueResolver.Resolve(container);
            actor.StateProvider.SetBoolean(type, isTrue);
            return UniTask.CompletedTask;
        }
    }
}
