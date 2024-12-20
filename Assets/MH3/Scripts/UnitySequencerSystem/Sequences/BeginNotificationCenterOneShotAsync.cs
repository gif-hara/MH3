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
    public class BeginNotificationCenterOneShotAsync : Sequence
    {
        [SerializeField]
        private HKUIDocument documentPrefab;

        [SerializeReference, SubclassSelector]
        private StringResolver messageResolver;

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var document = new UIViewNotificationCenter(documentPrefab, cancellationToken);
            var message = messageResolver?.Resolve(container) ?? string.Empty;
            if (string.IsNullOrEmpty(message))
            {
                await document.BeginOneShotAsync();
            }
            else
            {
                await document.BeginOneShotAsync(message);
            }
        }
    }
}
