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
            CanChangeState,
            CanAddFlinchDamage,
            Invincible,
            IsGuard,
            JustGuarding,
            CanGuard,
            IsEventStop,
        }

        public enum IntegerType
        {
            SuperArmorCount,
        }

        public enum TriggerType
        {
            OnFlinch,
            OnTakeDamage,
            OnDead,
            OnBeginAttack,
            OnEndAttack,
            OnReset,
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
                BooleanType.CanChangeState => actor.StateMachine.CanChangeState,
                BooleanType.CanAddFlinchDamage => actor.SpecController.CanAddFlinchDamage,
                BooleanType.Invincible => actor.SpecController.Invincible,
                BooleanType.IsGuard => actor.ActionController.IsGuard,
                BooleanType.JustGuarding => actor.ActionController.JustGuarding,
                BooleanType.CanGuard => actor.ActionController.CanGuard,
                BooleanType.IsEventStop => actor.SpecController.IsEventStop,
                _ => throw new ArgumentOutOfRangeException($"Unknown or Invalid type: {type}"),
            };
        }

        public Observable<int> GetIntegerAsObservable(IntegerType type)
        {
            return type switch
            {
                IntegerType.SuperArmorCount => actor.SpecController.SuperArmorCount,
                _ => throw new ArgumentOutOfRangeException($"Unknown or Invalid type: {type}"),
            };
        }

        public Observable<Unit> GetTriggerAsObservable(TriggerType type)
        {
            return type switch
            {
                TriggerType.OnFlinch => actor.SpecController.OnFlinch,
                TriggerType.OnTakeDamage => actor.SpecController.OnTakeDamage.Select(_ => Unit.Default),
                TriggerType.OnDead => actor.SpecController.OnDead,
                TriggerType.OnBeginAttack => actor.AttackController.OnBeginAttack,
                TriggerType.OnEndAttack => actor.AttackController.OnEndAttack,
                TriggerType.OnReset => actor.SpecController.OnReset,
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
                case BooleanType.CanChangeState:
                    actor.StateMachine.CanChangeState.Value = value;
                    break;
                case BooleanType.CanAddFlinchDamage:
                    actor.SpecController.CanAddFlinchDamage.Value = value;
                    break;
                case BooleanType.Invincible:
                    actor.SpecController.Invincible.Value = value;
                    break;
                case BooleanType.IsGuard:
                    actor.ActionController.IsGuard.Value = value;
                    break;
                case BooleanType.JustGuarding:
                    actor.ActionController.JustGuarding.Value = value;
                    break;
                case BooleanType.CanGuard:
                    actor.ActionController.CanGuard.Value = value;
                    break;
                case BooleanType.IsEventStop:
                    actor.SpecController.IsEventStop.Value = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown or Invalid type: {type}");
            }
        }

        public void SetInteger(IntegerType type, int value)
        {
            switch (type)
            {
                case IntegerType.SuperArmorCount:
                    actor.SpecController.SuperArmorCount.Value = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown or Invalid type: {type}");
            }
        }
    }
}
