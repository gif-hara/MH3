using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class RegisterContainerString : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver keyResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver valueResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var key = keyResolver.Resolve(container);
            var value = valueResolver.Resolve(container);
            container.Register(key, value);
            return UniTask.CompletedTask;
        }
    }
}
