using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class SetParentFromActor : Sequence
    {
        [SerializeReference, SubclassSelector]
        private TransformResolver targetTransformResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private Vector3Resolver positionResolver;

        [SerializeReference, SubclassSelector]
        private QuaternionResolver rotationResolver;

        [SerializeReference, SubclassSelector]
        private Vector3Resolver scaleResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var target = targetTransformResolver.Resolve(container);
            var actor = actorResolver.Resolve(container);
            target.SetParent(actor.transform);
            target.localPosition = positionResolver.Resolve(container);
            target.localRotation = rotationResolver.Resolve(container);
            target.localScale = scaleResolver.Resolve(container);
            return UniTask.CompletedTask;
        }
    }
}
