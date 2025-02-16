using System;
using R3;

namespace MH3
{
    public sealed class GameEvents
    {
        public readonly Subject<Unit> OnBeginTitle = new();

        public readonly Subject<Unit> OnEndTitle = new();

        public readonly Subject<Unit> OnBeginPauseMenu = new();

        public readonly Subject<Unit> OnEndPauseMenu = new();

        public readonly Subject<Unit> OnBeginQuestTransition = new();

        public readonly Subject<Unit> OnBeginAcquireReward = new();

        public readonly Subject<Unit> OnEndAcquireReward = new();

        public readonly Subject<Unit> OnTransitioned = new();

        public readonly Subject<Unit> OnBeginBattleStartEffect = new();

        public readonly Subject<Unit> OnEndBattleStartEffect = new();

        public enum Type
        {
            BeginTitle,
            EndTitle,
            BeginPauseMenu,
            EndPauseMenu,
            BeginQuestTransition,
            BeginAcquireReward,
            EndAcquireReward,
            Transitioned,
            BeginBattleStartEffect,
            EndBattleStartEffect,
        }

        public Subject<Unit> GetSubject(Type type)
        {
            return type switch
            {
                Type.BeginTitle => OnBeginTitle,
                Type.EndTitle => OnEndTitle,
                Type.BeginPauseMenu => OnBeginPauseMenu,
                Type.EndPauseMenu => OnEndPauseMenu,
                Type.BeginQuestTransition => OnBeginQuestTransition,
                Type.BeginAcquireReward => OnBeginAcquireReward,
                Type.EndAcquireReward => OnEndAcquireReward,
                Type.Transitioned => OnTransitioned,
                Type.BeginBattleStartEffect => OnBeginBattleStartEffect,
                Type.EndBattleStartEffect => OnEndBattleStartEffect,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }
}
