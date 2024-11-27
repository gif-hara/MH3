using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField]
        private Actor playerPrefab;
        
        private void Start()
        {
            var player = Instantiate(playerPrefab);
            var inputController = new InputController();
            PlayerController.Attach(player, inputController);
        }
    }
}
