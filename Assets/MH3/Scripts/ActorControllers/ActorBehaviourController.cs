using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorBehaviourController
    {
        private ActorBehaviourData data;
        
        public ActorBehaviourController(Actor actor, ActorBehaviourData data)
        {
            this.data = data;
        }
        
        public void ChangeBehaviour(ActorBehaviourData newData)
        {
            data = newData;
        }

        private async UniTaskVoid Begin(Actor actor)
        {
            try
            {
                while (!actor.destroyCancellationToken.IsCancellationRequested)
                {
                    var container = new Container();
                    container.Register("Actor", actor);
                    var sequencer = new Sequencer(container, data.EntryPoint.Sequences);
                    await sequencer.PlayAsync(actor.destroyCancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
