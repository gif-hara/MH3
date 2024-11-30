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

        public void ActiveAttackCollider(AttackData attackData)
        {
            actor.AttackController.SetActiveCollider(attackData.ColliderName, true);
        }

        public void DeactiveAllAttackCollider()
        {
            actor.AttackController.DeactiveAllAttackCollider();
        }
    }
}
