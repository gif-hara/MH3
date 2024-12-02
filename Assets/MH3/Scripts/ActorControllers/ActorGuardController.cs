using System.Threading;
using HK;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorGuardController
    {
        private readonly Actor actor;
        
        public readonly ReactiveProperty<bool> IsGuard = new(false);

        private CancellationTokenSource guardCancellationTokenSource;

        public ActorGuardController(Actor actor)
        {
            this.actor = actor;
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
        
        public bool IsSuccessGuard(Vector3 impactPosition)
        {
            if(!IsGuard.Value)
            {
                return false;
            }
            
            var forward = actor.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();
            impactPosition.y = 0.0f;
            var targetDirection = (impactPosition - actor.transform.position).normalized;
            var guardRange = TinyServiceLocator.Resolve<GameRules>().GuardRange;
            var guardAngle = Vector3.Dot(forward, targetDirection) * -1;
            return guardAngle > guardRange / 2.0f;
        }
    }
}
