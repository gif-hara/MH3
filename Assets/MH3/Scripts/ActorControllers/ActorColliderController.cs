using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorColliderController
    {
        public ActorColliderController(Actor actor)
        {
            actor.OnTriggerEnterAsObservable()
                .Subscribe(collider =>
                {
                    Debug.Log($"{actor.name}: OnTriggerEnter {collider.attachedRigidbody?.name}");
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
