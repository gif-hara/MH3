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
    public class SetParentFromLocator : Sequence
    {
        [SerializeReference, SubclassSelector]
        private TransformResolver targetTransformResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver locatorKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var target = targetTransformResolver.Resolve(container);
            var actor = actorResolver.Resolve(container);
            var locator = actor.LocatorHolder.Get(locatorKeyResolver.Resolve(container));
            target.SetParent(locator);
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
            return UniTask.CompletedTask;
        }
    }
}
