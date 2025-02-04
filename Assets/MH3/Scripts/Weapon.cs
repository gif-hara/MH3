using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    public class Weapon : MonoBehaviour, IActorOnTriggerEnterEvent
    {
        [SerializeField]
        private List<GameObject> colliders = new();

        [SerializeField]
        private TrailElement.DictionaryList trails = new();

        [SerializeReference, SubclassSelector]
        private SequencesResolver onSetupSequencesResolver;

        private Actor actor;

        public void Influence(Actor target, Collider collider)
        {
            var impactPosition = collider.ClosestPoint(transform.position);
            actor.AttackController.Attack(target, impactPosition, collider.name);
        }

        public void Setup(Actor actor)
        {
            this.actor = actor;
            gameObject.SetLayerRecursively(actor.GetAttackLayer());
            foreach (var collider in colliders)
            {
                actor.AttackController.AddCollider(collider.name, collider);
            }
            foreach (var trail in trails.List)
            {
                trail.Trail.Emit = false;
            }

            if (onSetupSequencesResolver != null)
            {
                var container = new Container();
                container.Register("Actor", actor);
                var sequences = onSetupSequencesResolver.Resolve(container);
                var sequencer = new Sequencer(container, sequences);
                sequencer.PlayAsync(actor.destroyCancellationToken).Forget();
            }
        }

        public void Dispose()
        {
            foreach (var collider in colliders)
            {
                actor.AttackController.RemoveCollider(collider.name);
            }
        }

        public void SetActiveTrail(string key, bool isActive)
        {
            if (trails.TryGetValue(key, out var trail))
            {
                trail.Trail.Emit = isActive;
            }
        }

        public void SetDeactiveAllTrail()
        {
            foreach (var trail in trails.List)
            {
                trail.Trail.Emit = false;
            }
        }

        [Serializable]
        public class ColliderElement
        {
            [SerializeField]
            private GameObject colliderObject;
            public GameObject ColliderObject => colliderObject;
        }

        [Serializable]
        public class TrailElement
        {
            [SerializeField]
            private MeleeWeaponTrail trail;
            public MeleeWeaponTrail Trail => trail;

            [Serializable]
            public class DictionaryList : DictionaryList<string, TrailElement>
            {
                public DictionaryList() : base(x => x.trail.name)
                {
                }
            }
        }
    }
}
