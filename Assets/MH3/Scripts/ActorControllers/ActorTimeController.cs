using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3
{
    public class ActorTimeController
    {
        private readonly Actor actor;

        public HK.Time Time { get; } = new HK.Time(HK.Time.Root);

        public Observable<Unit> UpdatedTimeScale => Observable.FromEvent(h => Time.UpdatedTimeScale += h, h => Time.UpdatedTimeScale -= h);

        private readonly Stack<float> timeScaleStack = new();

        public ActorTimeController(Actor actor)
        {
            this.actor = actor;
            PushTimeScale(1.0f);
        }

        public async UniTask BeginHitStopAsync(float timeScale, float duration)
        {
            PushTimeScale(timeScale);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: actor.destroyCancellationToken);
            PopTimeScale();
        }

        public void PushTimeScale(float timeScale)
        {
            timeScaleStack.Push(timeScale);
            Time.timeScale = timeScale;
        }

        public void PopTimeScale()
        {
            timeScaleStack.Pop();
            Time.timeScale = timeScaleStack.Peek();
        }
    }
}
