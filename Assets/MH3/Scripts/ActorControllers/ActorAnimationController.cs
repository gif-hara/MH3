using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorAnimationController
    {
        private readonly Actor actor;
        
        private readonly SimpleAnimation simpleAnimation;
        
        private SimpleAnimation.State currentCrossFadeState;

        public ActorAnimationController(Actor actor, SimpleAnimation simpleAnimation)
        {
            this.actor = actor;
            this.simpleAnimation = simpleAnimation;
        }

        public void CrossFade(string stateName, float fadeLength)
        {
            var state = simpleAnimation.GetState(stateName);
            if (!state.clip.isLooping)
            {
                state.normalizedTime = 0;
            }
            simpleAnimation.CrossFade(stateName, fadeLength);
        }
        
        public async UniTask WaitForAnimationEndAsync(string stateName, CancellationToken cancellationToken)
        {
            try
            {
                var state = simpleAnimation.GetState(stateName);
                while (state.normalizedTime < 1)
                {
                    await UniTask.NextFrame(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
