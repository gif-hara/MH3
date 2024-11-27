using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    public class PlayerController
    {
        public static void Attach(Actor actor, InputController inputController, ScriptableSequences attackSequence)
        {
            actor.UpdateAsObservable()
                .Subscribe((actor, inputController.Actions), static (_, t) =>
                {
                    var velocity = t.Actions.Player.Move.ReadValue<Vector2>();
                    t.actor.MovementController.Move(new Vector3(velocity.x, 0, velocity.y));
                })
                .RegisterTo(actor.destroyCancellationToken);
            inputController.Actions.Player.Attack.OnPerformedAsObservable()
                .Subscribe(actor, (_, a) =>
                {
                    a.StateMachine.TryChangeState(attackSequence);
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
