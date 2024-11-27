using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorMovementController
    {
        private Vector3 velocity;
        
        private Vector3 lastVelocity;

        public void Setup(Actor actor)
        {
            actor.UpdateAsObservable()
                .Subscribe(actor, (_, a) =>
                {
                    a.transform.position += velocity * Time.deltaTime;
                    lastVelocity = velocity;
                    velocity = Vector3.zero;
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
        
        public void Move(Vector3 velocity)
        {
            this.velocity = velocity;
        }
        
        public bool IsMoving()
        {
            return lastVelocity != Vector3.zero;
        }
    }
}
