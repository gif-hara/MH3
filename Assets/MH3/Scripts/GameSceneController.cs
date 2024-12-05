using HK;
using MH3.ActorControllers;
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
        private string defaultQuestSpecId;

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

        private Actor enemy;

        private Stage stage;

        private void Start()
        {
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(effectManagerPrefab), destroyCancellationToken).Forget();
            var gameCameraController = Instantiate(gameCameraControllerPrefab);
            TinyServiceLocator.RegisterAsync(gameCameraController, destroyCancellationToken).Forget();
            var player = masterData.ActorSpecs.Get(playerActorSpecId).Spawn(Vector3.zero, Quaternion.identity);
            SetupQuest(player, gameCameraController, defaultQuestSpecId);
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

        private void SetupQuest(Actor player, GameCameraController gameCameraController, string questSpecId)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
            
            if(stage != null)
            {
                Destroy(stage.gameObject);
            }
            
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            stage = Object.Instantiate(questSpec.StagePrefab);
            var enemySpec = TinyServiceLocator.Resolve<MasterData>().ActorSpecs.Get(questSpec.EnemyActorSpecId);
            enemy = enemySpec.Spawn(stage.EnemySpawnPoint.position, stage.EnemySpawnPoint.rotation);
            player.transform.position = stage.PlayerSpawnPoint.position;
            player.transform.rotation = stage.PlayerSpawnPoint.rotation;
            player.SpecController.Target.Value = enemy;
            enemy.SpecController.Target.Value = player;
            gameCameraController.SetTrackingTarget(player.transform, enemy.transform);
        }
    }
}
