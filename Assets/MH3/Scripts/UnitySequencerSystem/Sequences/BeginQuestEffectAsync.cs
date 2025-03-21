using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class BeginQuestEffectAsync : Sequence
    {
        [SerializeField]
        private HKUIDocument documentPrefab;

        [SerializeReference, SubclassSelector]
        private ActorResolver enemyResolver;


        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var enemy = enemyResolver.Resolve(container);
            var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await UIViewBeginQuest.OpenAsync(documentPrefab, enemy.SpecController.ActorName, scope.Token);
            scope.Cancel();
            scope.Dispose();
        }
    }
}
