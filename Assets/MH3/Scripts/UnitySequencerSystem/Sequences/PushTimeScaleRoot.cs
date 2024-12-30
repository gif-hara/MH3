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
    public class PushTimeScaleRoot : Sequence
    {
        [SerializeReference, SubclassSelector]
        private FloatResolver timeScaleResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            HK.Time.Root.PushTimeScale(timeScaleResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
