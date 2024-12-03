using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3
{
    public class PlayerController
    {
        public static void Attach(Actor actor, Transform cameraTransform)
        {
            var inputController = TinyServiceLocator.Resolve<InputController>();
            actor.UpdateAsObservable()
                .Subscribe((actor, inputController.Actions, cameraTransform), static (_, t) =>
                {
                    var (actor, actions, cameraTransform) = t;
                    var velocity = actions.Player.Move.ReadValue<Vector2>();
                    var cameraForward = cameraTransform.forward;
                    var cameraRight = cameraTransform.right;
                    cameraForward.y = 0;
                    cameraRight.y = 0;
                    cameraForward.Normalize();
                    cameraRight.Normalize();
                    var moveVector = (cameraForward.normalized * velocity.y + cameraRight.normalized * velocity.x).normalized;
                    actor.MovementController.Move(moveVector * actor.SpecController.MoveSpeed);
                    if (velocity != Vector2.zero)
                    {
                        actor.MovementController.Rotate(Quaternion.LookRotation(moveVector));
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Attack.OnPerformedAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    EarlyInputHandler.Invoke(() => a.AttackController.TryAttack(a.SpecController.Target.Value), TinyServiceLocator.Resolve<GameRules>().EarlyInputTime, a.destroyCancellationToken);
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Dodge.OnPerformedAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    EarlyInputHandler.Invoke(() => a.DodgeController.TryDodge(), TinyServiceLocator.Resolve<GameRules>().EarlyInputTime, a.destroyCancellationToken);
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Guard.OnPerformedAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    a.GuardController.IsGuard.Value = true;
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Guard.OnCanceledAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    a.GuardController.IsGuard.Value = false;
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
