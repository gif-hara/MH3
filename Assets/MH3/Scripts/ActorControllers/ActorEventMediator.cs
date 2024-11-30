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

        public void ActiveAttackCollider(string name)
        {
            actor.AttackController.SetActiveCollider(name, true);
        }

        public void DeactiveAttackCollider(string name)
        {
            actor.AttackController.SetActiveCollider(name, false);
        }
    }
}
