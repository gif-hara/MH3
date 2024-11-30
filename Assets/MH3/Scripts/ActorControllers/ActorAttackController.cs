using System.Collections.Generic;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorAttackController
    {
        private readonly Actor actor;

        private int attackCount = 0;

        private readonly Dictionary<string, GameObject> colliders = new();

        private AttackData attackData;

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

        public void SetAttackData(AttackData attackData)
        {
            this.attackData = attackData;
            SetActiveCollider(attackData.ColliderName, true);
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
            if (attackData == null)
            {
                Debug.LogError("AttackData is null.");
                return;
            }

            Debug.Log($"{actor.name} attack {target.name} with {attackData.Power} power.");
        }
    }
}
