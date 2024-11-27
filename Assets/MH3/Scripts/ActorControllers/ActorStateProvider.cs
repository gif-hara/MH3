using System;
using R3;

namespace MH3.ActorControllers
{
    public class ActorStateProvider
    {
        public enum BooleanStatusType
        {
            IsMoving,
        }

        private readonly Actor actor;
        
        public ActorStateProvider(Actor actor)
        {
            this.actor = actor;
        }

        public Observable<bool> GetBooleanStatusAsObservable(BooleanStatusType type)
        {
            return type switch
            {
                BooleanStatusType.IsMoving => actor.MovementController.IsMoving,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
