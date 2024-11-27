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
        
        public readonly ReactiveProperty<bool> CanChangeState = new(true);
        
        public ActorStateMachine(Actor actor, ScriptableSequences initialState)
        {
            this.actor = actor;
            TryChangeState(initialState);
        }
        
        public void Dispose()
        {
            stateMachine?.Dispose();
        }
        
        public bool TryChangeState(ScriptableSequences sequence)
        {
            if (!CanChangeState.Value)
            {
                return false;
            }
            
            stateSequences = sequence;
            stateMachine.Change(State);
            return true;
        }
        
        private async UniTask State(CancellationToken scope)
        {
            var container = new Container();
            container.Register("Actor", actor);
            var sequencer = new Sequencer(container, stateSequences.Sequences);
            await sequencer.PlayAsync(scope);
        }
    }
}
