using UnityEngine;

namespace MH3
{
    public class ActorTimeController
    {
        public HK.Time Time { get; } = new HK.Time(HK.Time.Root);
    }
}
