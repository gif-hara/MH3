using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class ActorEventMediator : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;

        public void Test()
        {
            Debug.Log("Test " + actor.name);
        }
    }
}
