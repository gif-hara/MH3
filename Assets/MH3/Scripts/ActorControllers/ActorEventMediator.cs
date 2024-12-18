using Cysharp.Threading.Tasks;
using HK;
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

        public void SetAttackSpec(string attackSpecId)
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

        public void SetRotateImmediateTargetRotation()
        {
            actor.MovementController.RotateImmediate(actor.MovementController.TargetRotation);
        }

        public void SetActiveTrail(string key)
        {
            actor.WeaponController.SetActiveTrail(key, true);
        }

        public void SetDeactiveTrail(string key)
        {
            actor.WeaponController.SetActiveTrail(key, false);
        }

        public void PlaySfx(string key)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(key);
        }

        public void Recovery()
        {
            actor.SpecController.RecoveryFromAnimation();
        }

        public void ResetAttackCount()
        {
            actor.AttackController.ResetAttackCount();
        }

        public void AddRecoveryCommandCount(int value)
        {
            actor.SpecController.AddRecoveryCommandCount(value);
        }

        public void BeginDualSwordDodgeMode()
        {
            actor.ActionController.BeginDualSwordDodgeModeAsync().Forget();
        }

        public void SetSuperArmor(int value)
        {
            actor.SpecController.SetSuperArmor(value);
        }

        public void BeginBladeEndurance()
        {
            actor.ActionController.BeginBladeEnduranceAsync().Forget();
        }
    }
}
