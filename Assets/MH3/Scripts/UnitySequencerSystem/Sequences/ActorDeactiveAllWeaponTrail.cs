using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorDeactiveAllWeaponTrail : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            actor.WeaponController.SetDeactiveAllTrail();
            return UniTask.CompletedTask;
        }
    }
}
