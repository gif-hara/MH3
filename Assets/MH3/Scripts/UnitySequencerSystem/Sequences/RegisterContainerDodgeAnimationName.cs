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
    public class RegisterContainerDodgeAnimationName : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver keyResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var key = keyResolver.Resolve(container);
            container.Register(key, actorResolver.Resolve(container).ActionController.GetDodgeAnimationName());
            return UniTask.CompletedTask;
        }
    }
}
