using UnityEngine;

namespace MH3.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        public ActorMovementController MovementController { get; private set; }

        void Awake()
        {
            MovementController = new ActorMovementController();
        }
    }
}
