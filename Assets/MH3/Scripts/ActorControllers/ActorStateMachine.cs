using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorStateMachine : IDisposable
    {
        private readonly TinyStateMachine stateMachine = new();

        private readonly Actor actor;

        private ScriptableSequences stateSequences;

        private ScriptableSequences nextStateSequences;

        public readonly ReactiveProperty<bool> CanChangeState = new(true);

        private Action<Container> containerAction;

        private Container nextContainer;

        public ActorStateMachine(Actor actor, ScriptableSequences initialState)
        {
            this.actor = actor;
            TryChangeState(initialState);
        }

        public void Dispose()
        {
            stateMachine?.Dispose();
        }

        public bool TryChangeState(ScriptableSequences sequence, bool force = false, Action<Container> containerAction = null, Container nextContainer = null)
        {
            if (!force)
            {
                if (nextStateSequences != null)
                {
                    return false;
                }
                if (!CanChangeState.Value)
                {
                    return false;
                }
                if (actor.SpecController.IsEventStop.Value)
                {
                    return false;
                }
            }

            nextStateSequences = sequence;
            this.nextContainer = nextContainer;
            this.containerAction = containerAction;
            stateMachine.Change(State);
            return true;
        }

        private async UniTask State(CancellationToken scope)
        {
            stateSequences = nextStateSequences;
            nextStateSequences = null;
            var container = this.nextContainer ?? new Container();
            nextContainer = null;
            container.RegisterOrReplace("Actor", actor);
            containerAction?.Invoke(container);
            containerAction = null;
            var sequencer = new Sequencer(container, stateSequences.Sequences);
            await sequencer.PlayAsync(scope);
        }

        public bool IsMatchState(ScriptableSequences sequence)
        {
            return stateSequences == sequence;
        }
    }
}
