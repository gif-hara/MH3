using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class Weapon : MonoBehaviour, IActorOnTriggerEnterEvent
    {
        [SerializeField]
        private List<ColliderElement> colliders = new();

        private Actor actor;

        public void Influence(Actor target, Collider collider)
        {
            actor.AttackController.Attack(target);
        }

        public async UniTaskVoid Setup(Actor actor)
        {
            this.actor = actor;
            gameObject.SetLayerRecursively(actor.GetAttackLayer());
            foreach (var collider in colliders)
            {
                actor.AttackController.AddCollider(collider.Name, collider.ColliderObject);
            }

            await UniTask.WaitUntilCanceled(destroyCancellationToken, completeImmediately: true);

            foreach (var collider in colliders)
            {
                actor.AttackController.RemoveCollider(collider.Name);
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
    }
}
