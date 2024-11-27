using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;

namespace MH3.ActorControllers
{
    public class ActorStateMachine : IDisposable
    {
        private readonly TinyStateMachine stateMachine = new();

        private readonly Actor actor;
        
        public ActorStateMachine(Actor actor)
        {
            this.actor = actor;
            stateMachine.Change(StateIdle);
        }
        
        public void Dispose()
        {
            stateMachine?.Dispose();
        }
        
        private UniTask StateIdle(CancellationToken scope)
        {
            actor.MovementController.IsMoving
                .Subscribe(this, (x, _this) =>
                {
                    if (x)
                    {
                        _this.stateMachine.Change(StateRun);
                    }
                })
                .RegisterTo(scope);
            return UniTask.CompletedTask;
        }
        
        private UniTask StateRun(CancellationToken scope)
        {
            actor.MovementController.IsMoving
                .Subscribe(this, (x, _this) =>
                {
                    if (!x)
                    {
                        _this.stateMachine.Change(StateIdle);
                    }
                })
                .RegisterTo(scope);
            return UniTask.CompletedTask;
        }
    }
}
