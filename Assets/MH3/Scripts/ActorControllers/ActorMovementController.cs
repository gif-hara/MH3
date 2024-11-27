using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorMovementController
    {
        private Vector3 velocity;

        private Quaternion rotation;

        private float rotationSpeed;
        
        private readonly ReactiveProperty<bool> isMoving = new(false);
        public ReadOnlyReactiveProperty<bool> IsMoving => isMoving;
        
        public readonly ReactiveProperty<bool> CanMove = new(true);

        public readonly ReactiveProperty<bool> CanRotate = new(true);

        public void Setup(Actor actor)
        {
            SetRotationSpeed(actor.SpecController.RotationSpeed);
            actor.UpdateAsObservable()
                .Subscribe(actor, (_, a) =>
                {
                    if (velocity == Vector3.zero || !CanMove.Value)
                    {
                        isMoving.Value = false;
                    }
                    else
                    {
                        a.transform.position += velocity * Time.deltaTime;
                        velocity = Vector3.zero;
                        isMoving.Value = true;
                    }

                    if (rotation == Quaternion.identity || CanRotate.Value)
                    {
                        a.transform.rotation = Quaternion.Slerp(a.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
        
        public void Move(Vector3 velocity)
        {
            this.velocity = velocity;
        }

        public void Rotate(Quaternion rotation)
        {
            this.rotation = rotation;
        }

        public void SetRotationSpeed(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
        }
    }
}
