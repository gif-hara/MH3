using HK;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorActionController
    {
        private readonly Actor actor;

        public readonly ReactiveProperty<bool> CanGuard = new(true);

        public readonly ReactiveProperty<bool> IsGuard = new(false);

        private readonly ReactiveProperty<float> beginGuardTime = new(0.0f);

        public readonly ReactiveProperty<bool> JustGuarding = new(false);

        public ActorActionController(Actor actor)
        {
            this.actor = actor;
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    if (!@this.CanGuard.Value)
                    {
                        return;
                    }
                    if (
                        @this.IsGuard.Value
                        && !actor.StateMachine.IsMatchState(actor.SpecController.GuardSequences)
                        && actor.StateMachine.TryChangeState(actor.SpecController.GuardSequences))
                    {
                        @this.beginGuardTime.Value = UnityEngine.Time.time;
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);

        }

        public bool TryDodge()
        {
            return actor.StateMachine.TryChangeState(
                actor.SpecController.DodgeSequences,
                containerAction: c =>
                {
                    c.Register("DodgeName", "Dodge");
                });
        }

        public Define.GuardResult GetGuardResult(Vector3 impactPosition)
        {
            if (!IsGuard.Value || !actor.StateMachine.IsMatchState(actor.SpecController.GuardSequences))
            {
                return Define.GuardResult.NotGuard;
            }

            var forward = actor.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();
            impactPosition.y = 0.0f;
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var targetDirection = actor.transform.position - impactPosition;
            targetDirection.y = 0.0f;
            targetDirection.Normalize();
            var guardRange = gameRules.GuardRange;
            var guardAngle = (1.0f - Vector3.Dot(forward, targetDirection) * -1) * 90.0f;
            var successGuard = guardAngle < guardRange / 2.0f;
            if (successGuard)
            {
                var guardTime = UnityEngine.Time.time - beginGuardTime.Value;
                return guardTime <= gameRules.JustGuardTime ? Define.GuardResult.SuccessJustGuard : Define.GuardResult.SuccessGuard;
            }
            else
            {
                return Define.GuardResult.FailedGuard;
            }
        }
    }
}