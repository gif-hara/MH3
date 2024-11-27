using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorMovementController
    {
        private Vector3 velocity;
        
        private readonly ReactiveProperty<bool> isMoving = new(false);
        public ReadOnlyReactiveProperty<bool> IsMoving => isMoving;

        public void Setup(Actor actor)
        {
            actor.UpdateAsObservable()
                .Subscribe(actor, (_, a) =>
                {
                    if (velocity == Vector3.zero)
                    {
                        isMoving.Value = false;
                    }
                    else
                    {
                        a.transform.position += velocity * Time.deltaTime;
                        velocity = Vector3.zero;
                        isMoving.Value = true;
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
        
        public void Move(Vector3 velocity)
        {
            this.velocity = velocity;
        }
    }
}
