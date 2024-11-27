using System.Collections.Generic;
using System.Threading;
using R3;
using R3.Triggers;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorActionController
    {
        public enum ActionType
        {
            Attack0,
        }

        private readonly Actor actor;
        
        private readonly Dictionary<ActionType, ScriptableSequences> actions = new();
        
        public readonly ReactiveProperty<bool> CanExecute = new(true);
        
        private CancellationTokenSource acceptScope;

        public ActorActionController(Actor actor)
        {
            this.actor = actor;
        }

        public void Accept(ActionType type)
        {
            acceptScope?.Cancel();
            acceptScope?.Dispose();
            if(!actions.ContainsKey(type))
            {
                return;
            }
            acceptScope = CancellationTokenSource.CreateLinkedTokenSource(actor.destroyCancellationToken);
            actor.UpdateAsObservable()
                .Subscribe(this, (_, _this) =>
                {
                    if (!_this.CanExecute.Value)
                    {
                        return;
                    }
                    
                    _this.actor.StateMachine.ChangeState(actions[type]);
                    _this.acceptScope?.Cancel();
                    _this.acceptScope?.Dispose();
                    _this.acceptScope = null;
                })
                .RegisterTo(acceptScope.Token);
        }

        public void ClearAction()
        {
            actions.Clear();
        }
        
        public void SetAction(ActionType type, ScriptableSequences sequence)
        {
            actions[type] = sequence;
        }
    }
}
