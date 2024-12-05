using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorBehaviourController
    {
        private readonly Actor actor;

        private ActorBehaviourData data;

        public ActorBehaviourController(Actor actor)
        {
            this.actor = actor;
        }

        public void ChangeBehaviour(ActorBehaviourData newData)
        {
            data = newData;
        }

        public async UniTaskVoid Begin(ActorBehaviourData data)
        {
            try
            {
                this.data = data;
                while (actor != null && !actor.destroyCancellationToken.IsCancellationRequested)
                {
                    var container = new Container();
                    container.Register("Actor", actor);
                    container.Register("Target", actor.SpecController.Target.Value);
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
