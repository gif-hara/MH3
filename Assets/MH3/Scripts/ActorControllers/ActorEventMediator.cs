using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class ActorEventMediator : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;

        public void SetCanChangeState(int value)
        {
            actor.StateMachine.CanChangeState.Value = value == 1;
        }

        public void SetAttackSpec(int attackSpecId)
        {
            actor.AttackController.SetAttackSpec(attackSpecId);
        }

        public void DeactiveAllAttackCollider()
        {
            actor.AttackController.DeactiveAllAttackCollider();
        }

        public void SetCanRotate(int value)
        {
            actor.MovementController.CanRotate.Value = value == 1;
        }

        public void SetInvinvible(int value)
        {
            actor.SpecController.Invincible.Value = value == 1;
        }
    }
}
