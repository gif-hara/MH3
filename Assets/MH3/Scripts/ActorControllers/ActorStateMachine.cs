using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorStateMachine : IDisposable
    {
        private readonly TinyStateMachine stateMachine = new();

        private readonly Actor actor;
        
        private ScriptableSequences stateSequences;
        
        
        public ActorStateMachine(Actor actor, ScriptableSequences initialState)
        {
            this.actor = actor;
            ChangeState(initialState);
        }
        
        public void Dispose()
        {
            stateMachine?.Dispose();
        }
        
        public void ChangeState(ScriptableSequences sequence)
        {
            stateSequences = sequence;
            stateMachine.Change(State);
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
