using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3
{
    public class ActorTimeController
    {
        public HK.Time Time { get; } = new HK.Time(HK.Time.Root);

        public Observable<Unit> UpdatedTimeScale => Observable.FromEvent(h => Time.UpdatedTimeScale += h, h => Time.UpdatedTimeScale -= h);

        public ActorTimeController(Actor actor)
        {
        }
    }
}
