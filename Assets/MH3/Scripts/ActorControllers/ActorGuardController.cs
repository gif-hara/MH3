using System.Threading;
using R3;
using R3.Triggers;

namespace MH3.ActorControllers
{
    public class ActorGuardController
    {
        public readonly ReactiveProperty<bool> IsGuard = new(false);

        private CancellationTokenSource guardCancellationTokenSource;

        public ActorGuardController(Actor actor)
        {
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    if (@this.IsGuard.Value && !actor.StateMachine.IsMatchState(actor.SpecController.GuardSequences))
                    {
                        actor.StateMachine.TryChangeState(actor.SpecController.GuardSequences);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
