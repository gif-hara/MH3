using MH3.ActorControllers;
using R3;
using R3.Triggers;

namespace MH3
{
    public class EnemyController
    {
        public static void Attach(Actor actor)
        {
            actor.UpdateAsObservable()
                .Subscribe(actor, (_, a) =>
                {
                    a.AttackController.TryAttack();
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
