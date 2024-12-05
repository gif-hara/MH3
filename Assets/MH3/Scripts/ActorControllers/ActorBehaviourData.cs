using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    [CreateAssetMenu(menuName = "MH3/ActorBehaviourData")]
    public class ActorBehaviourData : ScriptableObject
    {
        [SerializeField]
        private ScriptableSequences entryPoint;
        public ScriptableSequences EntryPoint => entryPoint;
    }
}
