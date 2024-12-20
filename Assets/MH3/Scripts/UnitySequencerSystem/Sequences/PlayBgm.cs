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
    public class PlayBgm : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver keyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlayBgm(keyResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
