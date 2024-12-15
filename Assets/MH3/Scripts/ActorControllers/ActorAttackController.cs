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

        private MasterData.AttackSpec attackSpec;

        private readonly HashSet<Actor> attackedActors = new();

        public bool HasNextCombo => attackCount < actor.SpecController.ComboAnimationKeys.Count;

        public ActorAttackController(Actor actor)
        {
            this.actor = actor;
        }

        public bool TryAttack(Actor target)
        {
            if (!string.IsNullOrEmpty(actor.SpecController.StrongAttackAnimationKey) && target.SpecController.FlinchType.CurrentValue == Define.FlinchType.Small)
            {
                if (actor.StateMachine.TryChangeState(
                    actor.SpecController.AttackSequences,
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
                    actor.SpecController.AttackSequences,
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
                    actor.SpecController.AttackSequences,
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
            attackedActors.Clear();
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

        public void Attack(Actor target, Vector3 impactPosition)
        {
            if (attackedActors.Contains(target))
            {
                return;
            }
            target.SpecController.TakeDamage(actor, attackSpec, impactPosition);
            attackedActors.Add(target);
        }
    }
}
