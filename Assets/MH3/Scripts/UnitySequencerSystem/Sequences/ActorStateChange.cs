using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class ActorStateChange : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private ScriptableSequences stateSequence;

        [SerializeReference, SubclassSelector]
        private BooleanResolver forceChangeResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver isSuccessKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var forceChange = forceChangeResolver.Resolve(container);
            var isSuccess = actor.StateMachine.TryChangeState(stateSequence, forceChange);
            if (isSuccessKeyResolver != null)
            {
                container.RegisterOrReplace(isSuccessKeyResolver.Resolve(container), isSuccess);
            }
            return UniTask.CompletedTask;
        }
    }
}
