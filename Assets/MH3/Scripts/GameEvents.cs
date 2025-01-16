using R3;

namespace MH3
{
    public sealed class GameEvents
    {
        public readonly Subject<Unit> OnBeginTitle = new();

        public readonly Subject<Unit> OnEndTitle = new();

        public readonly Subject<Unit> OnBeginPauseMenu = new();

        public readonly Subject<Unit> OnEndPauseMenu = new();
    }
}
