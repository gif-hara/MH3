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
    public class ActorBooleanStateObservable : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ActorStateProvider.BooleanStatusType type;
        
        [SerializeReference, SubclassSelector]
        private BooleanResolver isTrueResolver;
        
        [SerializeField]
        private ScriptableSequences subscribeSequence;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var isTrue = isTrueResolver.Resolve(container);
            actor.StateProvider.GetBooleanStatusAsObservable(type)
                .Subscribe(this, (x, _this) =>
                {
                    if (x == isTrue)
                    {
                        var sequencer = new Sequencer(container, _this.subscribeSequence.Sequences);
                        sequencer.PlayAsync(cancellationToken).Forget();
                    }
                })
                .RegisterTo(cancellationToken);
            return UniTask.CompletedTask;
        }
    }
}
