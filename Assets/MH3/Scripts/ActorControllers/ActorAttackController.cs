using System.Collections.Generic;
using HK;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorAttackController
    {
        private readonly Actor actor;

        private int attackCount = 0;

        private readonly Dictionary<string, GameObject> colliders = new();

        private readonly Dictionary<string, MasterData.AttackSpec> attackSpecs = new();

        private readonly HashSet<(Actor actor, string colliderName)> attackedActors = new();

        public bool HasNextCombo => attackCount < actor.SpecController.ComboAnimationKeys.Count;

        public ActorAttackController(Actor actor)
        {
            this.actor = actor;
        }

        public bool TryAttack(Actor target)
        {
            if (actor.SpecController.IsEventStop.Value)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(actor.SpecController.StrongAttackAnimationKey) && target.SpecController.FlinchType.CurrentValue == Define.FlinchType.Small)
            {
                if (actor.StateMachine.TryChangeState(
                    actor.SpecController.AttackStateSequences,
                    containerAction: c =>
                    {
                        c.Register("AttackName", actor.SpecController.StrongAttackAnimationKey);
                    }))
                {
                    return true;
                }
                return false;
            }
            if (actor.ActionController.JustGuarding.Value)
            {
                if (actor.StateMachine.TryChangeState(
                    actor.SpecController.AttackStateSequences,
                    containerAction: c =>
                    {
                        c.Register("AttackName", actor.SpecController.JustGuardAttackAnimationKey);
                    }))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (!HasNextCombo)
                {
                    return false;
                }

                if (actor.StateMachine.TryChangeState(
                    actor.SpecController.AttackStateSequences,
                    containerAction: c =>
                    {
                        c.Register("AttackName", actor.SpecController.ComboAnimationKeys[attackCount++]);
                    }))
                {
                    return true;
                }
                return false;
            }
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

        public void SetAttackSpec(string attackSpecId)
        {
            var attackSpec = TinyServiceLocator.Resolve<MasterData>().AttackSpecs.Get(attackSpecId);
            if (attackSpecs.ContainsKey(attackSpec.ColliderName))
            {
                attackSpecs.Remove(attackSpec.ColliderName);
                colliders[attackSpec.ColliderName].SetActive(false);
            }
            attackSpecs.Add(attackSpec.ColliderName, attackSpec);
            SetActiveCollider(attackSpec.ColliderName, true);
        }

        public void DeactiveAllAttackCollider()
        {
            foreach (var collider in colliders.Values)
            {
                collider.SetActive(false);
            }
            attackSpecs.Clear();
            attackedActors.Clear();
        }

        public void Attack(Actor target, Vector3 impactPosition, string colliderName)
        {
            if (attackedActors.Contains((target, colliderName)))
            {
                return;
            }
            target.SpecController.TakeDamage(actor, attackSpecs[colliderName], impactPosition);
            attackedActors.Add((target, colliderName));
        }
    }
}
