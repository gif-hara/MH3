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
    public class SetStaminaRecoveryRate : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private FloatResolver staminaRecoveryRateResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            actor.SpecController.StaminaRecoveryRate = staminaRecoveryRateResolver.Resolve(container);
            return UniTask.CompletedTask;
        }
    }
}
