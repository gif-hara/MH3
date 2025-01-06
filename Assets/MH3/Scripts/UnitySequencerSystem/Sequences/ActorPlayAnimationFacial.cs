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
    public class ActorPlayAnimationFacial : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver facialNameResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            actorResolver.Resolve(container).FacialController.Play(facialNameResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
