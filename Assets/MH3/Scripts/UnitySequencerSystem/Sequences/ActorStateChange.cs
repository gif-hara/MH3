using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorStateChange : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;
        
        [SerializeField]
        private ScriptableSequences stateSequence;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            actor.StateMachine.TryChangeState(stateSequence);
            return UniTask.CompletedTask;
        }
    }
}
