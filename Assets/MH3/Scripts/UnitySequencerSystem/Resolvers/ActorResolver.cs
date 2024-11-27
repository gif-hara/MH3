using System;
using MH3.ActorControllers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3.UnitySequencerSystem.Resolvers
{
    public abstract class ActorResolver : IResolver<Actor>
    {
        public abstract Actor Resolve(Container container);

#if USS_SUPPORT_SUB_CLASS_SELECTOR
        [AddTypeMenu("Reference")]
#endif
        [Serializable]
        public sealed class Reference : ActorResolver
        {
            [SerializeField]
            private Actor target;

            public Reference()
            {
            }

            public Reference(Actor target)
            {
                this.target = target;
            }

            public override Actor Resolve(Container container)
            {
                return target;
            }
        }

#if USS_SUPPORT_SUB_CLASS_SELECTOR
        [AddTypeMenu("Name")]
#endif
        [Serializable]
        public sealed class Name : ActorResolver
        {
            [SerializeField]
            private string name;

            public Name()
            {
            }

            public Name(string name)
            {
                this.name = name;
            }

            public override Actor Resolve(Container container)
            {
                return container.Resolve<Actor>(name);
            }
        }
    }
}