using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    public class SequencesMonoBehaviour : MonoBehaviour
    {
        [SerializeReference, SubclassSelector]
        private SequencesResolver sequencesResolver;

        [SerializeField]
        private bool playOnEnable;

        private void OnEnable()
        {
            if (playOnEnable)
            {
                PlayAsync(new Container(), destroyCancellationToken).Forget();
            }
        }

        public UniTask PlayAsync(Container container, CancellationToken scope)
        {
            container.Register("Root", transform);
            var sequences = sequencesResolver.Resolve(container);
            var sequencer = new Sequencer(container, sequences);
            return sequencer.PlayAsync(scope);
        }
    }
}
