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
    public class AddAvailableContents : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver keyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var userData = TinyServiceLocator.Resolve<UserData>();
            userData.AvailableContents.Add(keyResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
