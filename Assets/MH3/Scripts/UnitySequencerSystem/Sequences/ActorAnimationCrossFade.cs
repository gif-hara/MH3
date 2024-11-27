using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class ActorAnimationCrossFade : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;
        
        [SerializeReference, SubclassSelector]
        private StringResolver stateNameResolver;
        
        [SerializeReference, SubclassSelector]
        private FloatResolver fadeLengthResolver;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            actor.AnimationController.CrossFade(stateNameResolver.Resolve(container), fadeLengthResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
