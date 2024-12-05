using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class WaitForStateAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ScriptableSequences stateSequences;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            return UniTask.WaitUntil(() => actor.StateMachine.IsMatchState(stateSequences), PlayerLoopTiming.Update, cancellationToken);
        }
    }
}
