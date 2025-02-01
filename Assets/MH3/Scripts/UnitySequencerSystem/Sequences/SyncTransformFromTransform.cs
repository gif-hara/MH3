using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class SyncTransformFromTransform : Sequence
    {
        [SerializeReference, SubclassSelector]
        private TransformResolver targetTransformResolver;

        [SerializeReference, SubclassSelector]
        private TransformResolver syncTransformResolver;


        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var target = targetTransformResolver.Resolve(container);
            var sync = syncTransformResolver.Resolve(container);
            target.position = sync.position;
            target.rotation = sync.rotation;
            target.localScale = sync.localScale;
            return UniTask.CompletedTask;
        }
    }
}
