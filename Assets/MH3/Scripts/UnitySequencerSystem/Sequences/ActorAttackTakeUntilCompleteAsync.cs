using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using R3;
using R3.Triggers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorAttackTakeUntilCompleteAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver targetResolver;

        [SerializeField]
        private List<string> comboAnimationKeys;

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var scope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
            var actor = actorResolver.Resolve(container);
            var target = targetResolver.Resolve(container);
            actor.SpecController.ComboAnimationKeys.Clear();
            actor.SpecController.ComboAnimationKeys.AddRange(comboAnimationKeys);
            actor.UpdateAsObservable()
                .Subscribe((actor, target), static (_, t) =>
                {
                    var (actor, target) = t;
                    actor.AttackController.TryAttack(target);
                })
                .AddTo(scope.Token);
            await UniTask.WaitUntil(() => !actor.AttackController.HasNextCombo, PlayerLoopTiming.Update, cancellationToken);
            scope.Dispose();
        }
    }
}
