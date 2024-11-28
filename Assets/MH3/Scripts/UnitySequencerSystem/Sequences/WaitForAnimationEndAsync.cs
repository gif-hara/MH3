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
    public class WaitForAnimationEndAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;
        
        [SerializeReference, SubclassSelector]
        private StringResolver stateNameResolver;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            return actor.AnimationController.WaitForAnimationEndAsync(stateNameResolver.Resolve(container), actor.destroyCancellationToken);
        }
    }
}
