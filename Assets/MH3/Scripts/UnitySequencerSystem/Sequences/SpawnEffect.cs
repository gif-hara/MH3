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
    public class SpawnEffect : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver effectKeyResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver registerKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var effect = TinyServiceLocator.Resolve<EffectManager>().Rent(effectKeyResolver.Resolve(container));
            container.RegisterOrReplace(registerKeyResolver.Resolve(container), effect.transform);
            return UniTask.CompletedTask;
        }
    }
}
