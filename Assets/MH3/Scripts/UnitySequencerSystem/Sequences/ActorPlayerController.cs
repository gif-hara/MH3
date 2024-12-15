using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using R3;
using R3.Triggers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorPlayerController : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var inputController = TinyServiceLocator.Resolve<InputController>();
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            actor.UpdateAsObservable()
                .Subscribe((actor, inputController.Actions, gameCameraController), static (_, t) =>
                {
                    var (actor, actions, gameCameraController) = t;
                    var velocity = actions.Player.Move.ReadValue<Vector2>();
                    var cameraForward = gameCameraController.ControlledCamera.transform.forward;
                    var cameraRight = gameCameraController.ControlledCamera.transform.right;
                    cameraForward.y = 0;
                    cameraRight.y = 0;
                    cameraForward.Normalize();
                    cameraRight.Normalize();
                    var moveVector = (cameraForward.normalized * velocity.y + cameraRight.normalized * velocity.x).normalized;
                    actor.MovementController.Move(moveVector);
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
                    EarlyInputHandler.Invoke(() => a.ActionController.TryDodge(), TinyServiceLocator.Resolve<GameRules>().EarlyInputTime, a.destroyCancellationToken);
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Guard.OnPerformedAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    a.ActionController.IsGuard.Value = true;
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Guard.OnCanceledAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    a.ActionController.IsGuard.Value = false;
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Recovery.OnPerformedAsObservable()
                .Subscribe(actor, static (_, a) =>
                {
                    EarlyInputHandler.Invoke(() => a.SpecController.TryRecovery(), TinyServiceLocator.Resolve<GameRules>().EarlyInputTime, a.destroyCancellationToken);
                })
                .RegisterTo(actor.destroyCancellationToken);
            return UniTask.CompletedTask;
        }
    }
}
