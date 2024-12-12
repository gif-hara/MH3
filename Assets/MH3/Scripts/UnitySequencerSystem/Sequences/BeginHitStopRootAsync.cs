using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class BeginHitStopRootAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private FloatResolver durationResolver;

        [SerializeReference, SubclassSelector]
        private FloatResolver timeScaleResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            return HK.Time.Root.BeginHitStopAsync(durationResolver.Resolve(container), timeScaleResolver.Resolve(container), cancellationToken);
        }
    }
}
