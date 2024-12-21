using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    public abstract class EventHandlerSequencer : MonoBehaviour
    {
        [SerializeReference, SubclassSelector]
        private SequencesResolver sequencesResolver;

        protected void Play()
        {
            var container = new Container();
            var sequencer = new Sequencer(container, sequencesResolver.Resolve(container));
            sequencer.PlayAsync(destroyCancellationToken).Forget();
        }
    }
}
