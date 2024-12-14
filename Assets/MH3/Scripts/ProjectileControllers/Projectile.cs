using MH3.ActorControllers;
using UnityEngine;

namespace MH3.ProjectileControllers
{
    public class Projectile : MonoBehaviour, IActorOnTriggerEnterEvent
    {
        private Actor owner;

        private MasterData.AttackSpec attackSpec;

        public Projectile Spawn(Actor owner, MasterData.AttackSpec attackSpec, Vector3 position, Quaternion rotation)
        {
            var projectile = Instantiate(this, position, rotation);
            projectile.owner = owner;
            projectile.attackSpec = attackSpec;
            projectile.gameObject.SetLayerRecursively(owner.GetProjectileLayer());
            return projectile;
        }

        public void Influence(Actor target, Collider collider)
        {
            target.SpecController.TakeDamage(owner, attackSpec, collider.ClosestPoint(transform.position));
        }
    }
}
