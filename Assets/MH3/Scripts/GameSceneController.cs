using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField]
        private Actor playerPrefab;

        [SerializeField]
        private Transform playerSpawnPoint;

        [SerializeField]
        private Actor enemyPrefab;

        [SerializeField]
        private Transform enemySpawnPoint;
        
        [SerializeField]
        private MasterData masterData;

        [SerializeField]
        private GameCameraController gameCameraControllerPrefab;

        [SerializeField]
        private AudioManager audioManagerPrefab;
        
        private void Start()
        {
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            var player = Instantiate(playerPrefab);
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;
            var enemy = Instantiate(enemyPrefab);
            enemy.transform.position = enemySpawnPoint.position;
            enemy.transform.rotation = enemySpawnPoint.rotation;
            var gameCameraController = Instantiate(gameCameraControllerPrefab);
            gameCameraController.SetTrackingTarget(player.transform);
            PlayerController.Attach(player, gameCameraController.ControlledCamera.transform);
        }
    }
}
