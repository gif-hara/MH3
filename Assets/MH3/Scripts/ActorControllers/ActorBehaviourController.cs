using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorBehaviourController
    {
        private readonly Actor actor;

        private ActorBehaviourData data;

        private CancellationTokenSource behaviourScope;

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
                behaviourScope = CancellationTokenSource.CreateLinkedTokenSource(actor.destroyCancellationToken);
                while (actor != null && !behaviourScope.IsCancellationRequested)
                {
                    var container = new Container();
                    container.Register("Actor", actor);
                    container.Register("Target", actor.SpecController.Target.Value);
                    var sequencer = new Sequencer(container, this.data.EntryPoint.Sequences);
                    await sequencer.PlayAsync(behaviourScope.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("ActorBehaviourController Begin Canceled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
