using MH3.ActorControllers;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3
{
    public class EnemyController
    {
        public static void Attach(Actor actor, Actor target)
        {
            actor.UpdateAsObservable()
                .Subscribe((actor, target), (_, t) =>
                {
                    var (actor, target) = t;
                    var lookAt = target.transform.position - actor.transform.position;
                    lookAt.y = 0.0f;
                    actor.MovementController.Rotate(Quaternion.LookRotation(lookAt));
                    actor.AttackController.TryAttack();
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
