using R3;
using R3.Triggers;

namespace MH3.ActorControllers
{
    public class ActorColliderController
    {
        public ActorColliderController(Actor actor)
        {
            actor.OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    var actorOnTriggerEnterEvents = collider.attachedRigidbody?.GetComponents<IActorOnTriggerEnterEvent>();
                    if (actorOnTriggerEnterEvents == null)
                    {
                        return;
                    }
                    foreach (var triggerEnterEvent in actorOnTriggerEnterEvents)
                    {
                        triggerEnterEvent.Influence(actor);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
