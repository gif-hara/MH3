using System;
using R3;

namespace MH3.ActorControllers
{
    public class ActorStateProvider
    {
        public enum BooleanType
        {
            IsMoving,
            CanMove,
            CanRotate,
        }

        private readonly Actor actor;
        
        public ActorStateProvider(Actor actor)
        {
            this.actor = actor;
        }

        public Observable<bool> GetBooleanAsObservable(BooleanType type)
        {
            return type switch
            {
                BooleanType.IsMoving => actor.MovementController.IsMoving,
                BooleanType.CanMove => actor.MovementController.CanMove,
                BooleanType.CanRotate => actor.MovementController.CanRotate,
                _ => throw new ArgumentOutOfRangeException($"Unknown or Invalid type: {type}"),
            };
        }
        
        public void SetBoolean(BooleanType type, bool value)
        {
            switch (type)
            {
                case BooleanType.CanMove:
                    actor.MovementController.CanMove.Value = value;
                    break;
                case BooleanType.CanRotate:
                    actor.MovementController.CanRotate.Value = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown or Invalid type: {type}");
            }
        }
    }
}
