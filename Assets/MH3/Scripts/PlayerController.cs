using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class PlayerController
    {
        public static void Attach(Actor actor)
        {
            Debug.Log($"PlayerController.Attach: {actor}");
        }
    }
}
