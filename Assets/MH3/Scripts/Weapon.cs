using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class Weapon : MonoBehaviour, IActorOnTriggerEnterEvent
    {
        [SerializeField]
        private List<ColliderElement> colliders = new();

        [SerializeField]
        private TrailElement.DictionaryList trails = new();

        private Actor actor;

        public void Influence(Actor target, Collider collider)
        {
            var impactPosition = collider.ClosestPoint(transform.position);
            actor.AttackController.Attack(target, impactPosition);
        }

        public void Setup(Actor actor)
        {
            this.actor = actor;
            gameObject.SetLayerRecursively(actor.GetAttackLayer());
            foreach (var collider in colliders)
            {
                actor.AttackController.AddCollider(collider.Name, collider.ColliderObject);
            }
            foreach (var trail in trails.List)
            {
                trail.Trail.Emit = false;
            }
        }

        public void Dispose()
        {
            foreach (var collider in colliders)
            {
                actor.AttackController.RemoveCollider(collider.Name);
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
            private string name;
            public string Name => name;

            [SerializeField]
            private GameObject colliderObject;
            public GameObject ColliderObject => colliderObject;
        }

        [Serializable]
        public class TrailElement
        {
            [SerializeField]
            private string key;
            public string Key => key;

            [SerializeField]
            private MeleeWeaponTrail trail;
            public MeleeWeaponTrail Trail => trail;

            [Serializable]
            public class DictionaryList : DictionaryList<string, TrailElement>
            {
                public DictionaryList() : base(x => x.Key)
                {
                }
            }
        }
    }
}
