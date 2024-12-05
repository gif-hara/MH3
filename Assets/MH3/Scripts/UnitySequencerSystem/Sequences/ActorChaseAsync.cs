using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using R3;
using R3.Triggers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class ActorChaseAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;
        
        [SerializeReference, SubclassSelector]
        private ActorResolver targetResolver;
        
        [SerializeReference, SubclassSelector]
        private BooleanResolver conditionResolver;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            using var chaseScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));
            var actor = actorResolver.Resolve(container);
            var target = targetResolver.Resolve(container);
            actor.UpdateAsObservable()
                .Subscribe((actor, target), static (_, t) =>
                {
                    var (actor, target) = t;
                    var direction = target.transform.position - actor.transform.position;
                    direction.y = 0;
                    actor.MovementController.Move(direction);
                    actor.MovementController.Rotate(Quaternion.LookRotation(direction));
                })
                .RegisterTo(chaseScope.Token);
            return UniTask.WaitUntil(() => conditionResolver.Resolve(container), PlayerLoopTiming.Update, cancellationToken);
        }
    }
}
