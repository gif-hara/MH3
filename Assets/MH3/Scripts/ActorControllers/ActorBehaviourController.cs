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
                if (behaviourScope != null)
                {
                    behaviourScope.Cancel();
                    behaviourScope.Dispose();
                }
                this.data = data;
                behaviourScope = CancellationTokenSource.CreateLinkedTokenSource(actor.destroyCancellationToken, actor.SpecController.DeadCancellationToken);
                foreach (var i in this.data.TriggerElements)
                {
                    actor.StateProvider.GetTriggerAsObservable(i.TriggerType)
                        .Subscribe((actor, i, behaviourScope), async static (_, t) =>
                        {
                            var (actor, i, behaviourScope) = t;
                            var container = new Container();
                            container.Register("Actor", actor);
                            container.Register("Target", actor.SpecController.Target.Value);
                            var sequencer = new Sequencer(container, i.Sequences.Sequences);
                            await sequencer.PlayAsync(behaviourScope.Token);
                        })
                        .RegisterTo(behaviourScope.Token);
                }
                while (actor != null && behaviourScope != null && !behaviourScope.IsCancellationRequested)
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

        public void Reset()
        {
            Begin(data).Forget();
        }

        public void Stop()
        {
            if (behaviourScope != null)
            {
                behaviourScope.Cancel();
                behaviourScope.Dispose();
            }
            behaviourScope = null;
        }
    }
}
