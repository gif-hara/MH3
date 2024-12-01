using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace MH3.ActorControllers
{
    public class ActorAnimationController
    {
        private readonly Actor actor;

        private readonly SimpleAnimation simpleAnimation;

        public ActorAnimationController(Actor actor, SimpleAnimation simpleAnimation)
        {
            this.actor = actor;
            this.simpleAnimation = simpleAnimation;
        }

        public void CrossFade(string stateName, float fadeLength)
        {
            var state = simpleAnimation.GetState(stateName);
            Assert.IsNotNull(state, $"State {stateName} not found");
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
