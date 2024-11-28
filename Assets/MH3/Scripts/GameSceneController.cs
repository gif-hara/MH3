using HK;
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
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            PlayerController.Attach(player, attackSequence);
        }
    }
}
