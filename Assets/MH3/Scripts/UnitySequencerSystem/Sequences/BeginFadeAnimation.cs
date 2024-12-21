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
    public class BeginFadeAnimation : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver animationKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<UIViewFade>().BeginAnimation(animationKeyResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
