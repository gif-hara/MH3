using R3;
using UnityEngine;

namespace MH3
{
    public static class GameEvents
    {
        public static readonly Subject<Unit> OnBeginTitle = new();
    }
}
