using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorAttackController
    {
        private readonly Actor actor;

        private int attackCount = 0;

        private readonly Dictionary<string, GameObject> colliders = new();

        private MasterData.AttackSpec attackSpec;

        private readonly string[] attackNames = new string[]
        {
            "Attack.0",
            "Attack.1",
            "Attack.0",
            "Attack.1",
        };

        public ActorAttackController(Actor actor)
        {
            this.actor = actor;
        }

        public bool TryAttack()
        {
            if (attackCount >= attackNames.Length)
            {
                return false;
            }

            if (actor.StateMachine.TryChangeState(
                actor.SpecController.AttackSequences,
                containerAction: c =>
                {
                    c.Register("AttackName", attackNames[attackCount++]);
                }))
            {
                return true;
            }

            return false;
        }

        public void ResetAttackCount()
        {
            attackCount = 0;
        }

        public void AddCollider(string name, GameObject collider)
        {
            colliders.Add(name, collider);
            SetActiveCollider(name, false);
        }

        public void RemoveCollider(string name)
        {
            colliders.Remove(name);
        }

        public void SetActiveCollider(string name, bool active)
        {
            if (colliders.TryGetValue(name, out var collider))
            {
                collider.SetActive(active);
            }
            else
            {
                Debug.LogError($"Collider {name} not found.");
            }
        }

        public void SetAttackSpec(int attackSpecId)
        {
            attackSpec = TinyServiceLocator.Resolve<MasterData>().AttackSpecs.Get(attackSpecId);
            SetActiveCollider(attackSpec.ColliderName, true);
        }

        public void DeactiveAllAttackCollider()
        {
            foreach (var collider in colliders.Values)
            {
                collider.SetActive(false);
            }
        }

        public void Attack(Actor target)
        {
            if (attackSpec == null)
            {
                Debug.LogError("AttackSpec is null.");
                return;
            }
            if (target.SpecController.Invincible.Value)
            {
                return;
            }
            var damageData = new DamageData(attackSpec.Power, attackSpec.FlinchDamage);
            target.SpecController.TakeDamage(damageData);
            actor.TimeController.BeginHitStopAsync(attackSpec.HitStopTimeScaleActor, attackSpec.HitStopDurationActor).Forget();
            target.TimeController.BeginHitStopAsync(attackSpec.HitStopTimeScaleTarget, attackSpec.HitStopDurationTarget).Forget();
            if (target.SpecController.CanPlayFlinch())
            {
                var lookAt = actor.transform.position - target.transform.position;
                lookAt.y = 0.0f;
                target.MovementController.RotateImmediate(Quaternion.LookRotation(lookAt));
                target.MovementController.CanRotate.Value = false;
                target.StateMachine.TryChangeState(target.SpecController.FlinchSequences, force: true, containerAction: c => c.Register("FlinchName", attackSpec.FlinchName));
                target.SpecController.ResetFlinch();
            }
        }
    }
}
