using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ProjectileControllers;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class SpawnProjectile : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeReference, SubclassSelector]
        private StringResolver attackSpecIdResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver registerKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var attackSpecId = attackSpecIdResolver.Resolve(container);
            var projectile = projectilePrefab.Spawn(actor, TinyServiceLocator.Resolve<MasterData>().AttackSpecs.Get(attackSpecId), actor.transform.position, actor.transform.rotation);
            container.Register(registerKeyResolver.Resolve(container), projectile.transform);
            return UniTask.CompletedTask;
        }
    }
}
