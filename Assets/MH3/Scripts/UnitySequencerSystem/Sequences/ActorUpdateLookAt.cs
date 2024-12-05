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
    public class ActorUpdateLookAt : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private ActorResolver targetResolver;

        [SerializeReference, SubclassSelector]
        private FloatResolver offsetResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var target = targetResolver.Resolve(container);
            actor.UpdateAsObservable()
                .Subscribe((actor, target, offsetResolver, container), static (_, t) =>
                {
                    var (actor, target, offsetResolver, container) = t;
                    var direction = target.transform.position - actor.transform.position;
                    direction.y = 0;
                    direction.Normalize();
                    actor.MovementController.Rotate(Quaternion.LookRotation(direction) * Quaternion.Euler(0, offsetResolver.Resolve(container), 0));
                })
                .RegisterTo(cancellationToken);
            return UniTask.CompletedTask;
        }
    }
}
