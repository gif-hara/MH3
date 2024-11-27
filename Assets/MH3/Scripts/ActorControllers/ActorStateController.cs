using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorStateController : IDisposable
    {
        private readonly TinyStateMachine stateMachine = new();

        private readonly Actor actor;
        
        public ActorStateController(Actor actor)
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
