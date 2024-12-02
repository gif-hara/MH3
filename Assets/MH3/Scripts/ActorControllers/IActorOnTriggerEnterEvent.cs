using UnityEngine;

namespace MH3.ActorControllers
{
    public interface IActorOnTriggerEnterEvent
    {
        void Influence(Actor target, Collider collider);
    }
}
