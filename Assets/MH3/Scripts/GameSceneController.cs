using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnitySequencerSystem;

namespace MH3
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField]
        private int playerActorSpecId;

        [SerializeField]
        private string homeQuestSpecId;

        [SerializeField]
        private string initialQuestSpecId;

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

        [SerializeField]
        private HKUIDocument damageLabelDocumentPrefab;

        private Actor enemy;

        private Stage stage;

        private UIViewDamageLabel damageLabel;

        private CancellationDisposable questScope;

        private void Start()
        {
            TinyServiceLocator.RegisterAsync(new InputController(), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(effectManagerPrefab), destroyCancellationToken).Forget();
            var gameCameraController = Instantiate(gameCameraControllerPrefab);
            TinyServiceLocator.RegisterAsync(gameCameraController, destroyCancellationToken).Forget();
            var playerSpec = masterData.ActorSpecs.Get(playerActorSpecId);
            var player = playerSpec.Spawn(Vector3.zero, Quaternion.identity);
            player.BehaviourController.Begin(playerSpec.Behaviour).Forget();
            _ = new UIViewPlayerStatus(playerStatusDocumentPrefab, player, destroyCancellationToken);
            damageLabel = new UIViewDamageLabel(damageLabelDocumentPrefab, gameCameraController.ControlledCamera, destroyCancellationToken);
            SetupQuest(player, gameCameraController, initialQuestSpecId);
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
                    if (Keyboard.current.f5Key.wasPressedThisFrame)
                    {
                        player.SpecController.SetHitPointDebug(1);
                        Debug.Log($"Player HitPoint: {player.SpecController.HitPoint.CurrentValue}");
                    }
                    if (Keyboard.current.f6Key.wasPressedThisFrame)
                    {
                        enemy.SpecController.SetHitPointDebug(1);
                        Debug.Log($"Enemy HitPoint: {enemy.SpecController.HitPoint.CurrentValue}");
                    }
                });
#endif
        }

        private void SetupQuest(Actor player, GameCameraController gameCameraController, string questSpecId)
        {
            if (questScope != null)
            {
                questScope.Dispose();
            }
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }

            if (stage != null)
            {
                Destroy(stage.gameObject);
            }

            questScope = new CancellationDisposable();
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            stage = Object.Instantiate(questSpec.StagePrefab);
            var enemySpec = TinyServiceLocator.Resolve<MasterData>().ActorSpecs.Get(questSpec.EnemyActorSpecId);
            enemy = enemySpec.Spawn(stage.EnemySpawnPoint.position, stage.EnemySpawnPoint.rotation);
            player.transform.position = stage.PlayerSpawnPoint.position;
            player.transform.rotation = stage.PlayerSpawnPoint.rotation;
            player.SpecController.Target.Value = enemy;
            enemy.SpecController.Target.Value = player;
            enemy.BehaviourController.Begin(enemySpec.Behaviour).Forget();
            gameCameraController.SetTrackingTarget(player.transform, enemy.transform);
            damageLabel.BeginObserve(enemy);
            var questClearContainer = new Container();
            questClearContainer.Register("Player", player);
            questClearContainer.Register("Enemy", enemy);
            var questClearSequencer = new Sequencer(questClearContainer, questSpec.QuestClearSequences.Sequences);
            questClearSequencer.PlayAsync(questScope.Token).Forget();
        }
    }
}
