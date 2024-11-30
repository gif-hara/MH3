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

        [SerializeField]
        private MasterData masterData;

        [SerializeField]
        private GameCameraController gameCameraControllerPrefab;
        
        private void Start()
        {
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            var player = Instantiate(playerPrefab);
            var gameCameraController = Instantiate(gameCameraControllerPrefab);
            gameCameraController.SetTrackingTarget(player.transform);
            PlayerController.Attach(player, attackSequence);
        }
    }
}
