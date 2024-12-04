using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [SerializeField]
        private EffectManager effectManagerPrefab;

        [SerializeField]
        private HKUIDocument playerStatusDocumentPrefab;

        private void Start()
        {
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(effectManagerPrefab), destroyCancellationToken).Forget();
            var player = masterData.ActorSpecs.Get(playerActorSpecId).Spawn(playerSpawnPoint.position, playerSpawnPoint.rotation);
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;
            var enemy = masterData.ActorSpecs.Get(enemyActorSpecId).Spawn(enemySpawnPoint.position, enemySpawnPoint.rotation);
            enemy.transform.position = enemySpawnPoint.position;
            enemy.transform.rotation = enemySpawnPoint.rotation;
            var gameCameraController = Instantiate(gameCameraControllerPrefab);
            gameCameraController.SetTrackingTarget(player.transform, enemy.transform);
            player.SpecController.Target.Value = enemy;
            enemy.SpecController.Target.Value = player;
            PlayerController.Attach(player, gameCameraController.ControlledCamera.transform);
            EnemyController.Attach(enemy, player);
            new UIViewPlayerStatus(playerStatusDocumentPrefab, player, destroyCancellationToken);
#if DEBUG
            var debugData = new GameDebugData();
            TinyServiceLocator.RegisterAsync(debugData, destroyCancellationToken).Forget();
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (Keyboard.current.f1Key.wasPressedThisFrame)
                    {
                        debugData.InvinciblePlayer = !debugData.InvinciblePlayer;
                        Debug.Log($"InvinciblePlayer: {debugData.InvinciblePlayer}");
                    }
                    if (Keyboard.current.f2Key.wasPressedThisFrame)
                    {
                        debugData.InvincibleEnemy = !debugData.InvincibleEnemy;
                        Debug.Log($"InvincibleEnemy: {debugData.InvincibleEnemy}");
                    }
                    if (Keyboard.current.f3Key.wasPressedThisFrame)
                    {
                        debugData.DamageZeroPlayer = !debugData.DamageZeroPlayer;
                        Debug.Log($"DamageZeroPlayer: {debugData.DamageZeroPlayer}");
                    }
                    if (Keyboard.current.f4Key.wasPressedThisFrame)
                    {
                        debugData.DamageZeroEnemy = !debugData.DamageZeroEnemy;
                        Debug.Log($"DamageZeroEnemy: {debugData.DamageZeroEnemy}");
                    }
                });
#endif
        }
    }
}
