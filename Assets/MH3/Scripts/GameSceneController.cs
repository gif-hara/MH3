using HK;
using UnityEngine;

namespace MH3
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField]
        private int playerActorSpecId;

        [SerializeField]
        private Transform playerSpawnPoint;

        [SerializeField]
        private int enemyActorSpecId;

        [SerializeField]
        private Transform enemySpawnPoint;

        [SerializeField]
        private MasterData masterData;
        
        [SerializeField]
        private GameRules gameRules;

        [SerializeField]
        private GameCameraController gameCameraControllerPrefab;

        [SerializeField]
        private AudioManager audioManagerPrefab;

        private void Start()
        {
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            var player = masterData.ActorSpecs.Get(playerActorSpecId).Spawn(playerSpawnPoint.position, playerSpawnPoint.rotation);
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;
            var enemy = masterData.ActorSpecs.Get(enemyActorSpecId).Spawn(enemySpawnPoint.position, enemySpawnPoint.rotation);
            enemy.transform.position = enemySpawnPoint.position;
            enemy.transform.rotation = enemySpawnPoint.rotation;
            var gameCameraController = Instantiate(gameCameraControllerPrefab);
            gameCameraController.SetTrackingTarget(player.transform, enemy.transform);
            PlayerController.Attach(player, gameCameraController.ControlledCamera.transform);
            EnemyController.Attach(enemy, player);
        }
    }
}
