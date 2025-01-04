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
    public class TrySharpen : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver isSuccessKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var isSuccess = actor.ActionController.TrySharpen();
            if (isSuccessKeyResolver != null)
            {
                container.RegisterOrReplace(isSuccessKeyResolver.Resolve(container), isSuccess);
            }

            return UniTask.CompletedTask;
        }
    }
}
