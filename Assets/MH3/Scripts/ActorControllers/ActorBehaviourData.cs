using System;
using System.Collections.Generic;
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

        [SerializeField]
        private List<TriggerElement> triggerElements;
        public List<TriggerElement> TriggerElements => triggerElements;

        [Serializable]
        public class TriggerElement
        {
            [SerializeField]
            private ActorStateProvider.TriggerType triggerType;
            public ActorStateProvider.TriggerType TriggerType => triggerType;

            [SerializeField]
            private ScriptableSequences sequences;
            public ScriptableSequences Sequences => sequences;
        }
    }
}
