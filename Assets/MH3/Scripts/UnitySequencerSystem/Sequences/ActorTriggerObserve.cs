using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.ActorControllers;
using MH3.UnitySequencerSystem.Resolvers;
using R3;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class ActorTriggerObserve : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ActorStateProvider.TriggerType triggerType;

        [SerializeReference, SubclassSelector]
        private SequencesResolver sequences;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, actor.destroyCancellationToken);
            actor.StateProvider.GetTriggerAsObservable(triggerType)
                .Subscribe((actor, this, cancellationToken, container), async static (_, t) =>
                {
                    var (actor, self, cancellationToken, container) = t;
                    var sequencer = new Sequencer(container, self.sequences.Resolve(container));
                    await sequencer.PlayAsync(cancellationToken);
                })
                .RegisterTo(scope.Token);
            return UniTask.CompletedTask;
        }
    }
}
