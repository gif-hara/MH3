using MH3.ActorControllers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField]
        private Actor playerPrefab;
        
        [SerializeField]
        private ScriptableSequences attackSequence;
        
        private void Start()
        {
            var player = Instantiate(playerPrefab);
            var inputController = new InputController();
            PlayerController.Attach(player, inputController, attackSequence);
        }
    }
}
