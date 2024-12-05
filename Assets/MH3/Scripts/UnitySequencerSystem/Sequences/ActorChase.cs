using System;
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
    public class ActorChase : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver targetResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var target = targetResolver.Resolve(container);
            actor.UpdateAsObservable()
                .Subscribe((actor, target), static (_, t) =>
                {
                    var (actor, target) = t;
                    var direction = target.transform.position - actor.transform.position;
                    direction.y = 0;
                    direction.Normalize();
                    actor.MovementController.Move(direction);
                    actor.MovementController.Rotate(Quaternion.LookRotation(direction));
                })
                .RegisterTo(cancellationToken);
            return UniTask.CompletedTask;
        }
    }
}
