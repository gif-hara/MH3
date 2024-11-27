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
        
        private ScriptableSequences currentStateSequence;
        
        private ScriptableSequences nextStateSequence;
        
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
            nextStateSequence = sequence;
            stateMachine.Change(State);
        }
        
        private async UniTask State(CancellationToken scope)
        {
            currentStateSequence = nextStateSequence;
            var container = new Container();
            container.Register("Actor", actor);
            var sequencer = new Sequencer(container, currentStateSequence.Sequences);
            await sequencer.PlayAsync(scope);
            currentStateSequence = null;
        }
    }
}
