using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

        [SerializeField]
        private HKUIDocument listDocumentPrefab;

        [SerializeField]
        private HKUIDocument headerDocumentPrefab;

        private Actor player;

        private Actor enemy;

        private Stage stage;

        private GameCameraController gameCameraController;

        private UIViewDamageLabel damageLabel;

        private CancellationDisposable questScope;

        private void Start()
        {
            var inputController = new InputController();
            TinyServiceLocator.RegisterAsync(inputController, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(effectManagerPrefab), destroyCancellationToken).Forget();
            gameCameraController = Instantiate(gameCameraControllerPrefab);
            TinyServiceLocator.RegisterAsync(gameCameraController, destroyCancellationToken).Forget();
            var playerSpec = masterData.ActorSpecs.Get(playerActorSpecId);
            player = playerSpec.Spawn(Vector3.zero, Quaternion.identity);
            player.BehaviourController.Begin(playerSpec.Behaviour).Forget();
            _ = new UIViewPlayerStatus(playerStatusDocumentPrefab, player, destroyCancellationToken);
            damageLabel = new UIViewDamageLabel(damageLabelDocumentPrefab, gameCameraController.ControlledCamera, destroyCancellationToken);
            SetupQuest(initialQuestSpecId);
            inputController.Actions.Player.PauseMenu
                .OnPerformedAsObservable()
                .Subscribe(_ =>
                {
                    UIViewPauseMenu.OpenAsync(headerDocumentPrefab, listDocumentPrefab, player, destroyCancellationToken).Forget();
                })
                .RegisterTo(destroyCancellationToken);
#if DEBUG
            var debugData = new GameDebugData();
            var isOpenDebugMenu = false;
            TinyServiceLocator.RegisterAsync(debugData, destroyCancellationToken).Forget();
            this.UpdateAsObservable()
                .Subscribe(async _ =>
                {
                    if (Keyboard.current.escapeKey.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed && !isOpenDebugMenu)
                    {
                        isOpenDebugMenu = true;
                        await UIViewDebugMenu.OpenAsync(
                            headerDocumentPrefab,
                            listDocumentPrefab,
                            this,
                            player,
                            enemy,
                            destroyCancellationToken
                            );
                        isOpenDebugMenu = false;
                    }
                });
#endif
        }

        private void SetupQuest(string questSpecId)
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
            stage = UnityEngine.Object.Instantiate(questSpec.StagePrefab);
            var enemySpec = TinyServiceLocator.Resolve<MasterData>().ActorSpecs.Get(questSpec.EnemyActorSpecId);
            enemy = enemySpec.Spawn(stage.EnemySpawnPoint.position, stage.EnemySpawnPoint.rotation);
            player.SpecController.ResetAll();
            player.transform.position = stage.PlayerSpawnPoint.position;
            player.MovementController.RotateImmediate(stage.PlayerSpawnPoint.rotation);
            player.SpecController.Target.Value = enemy;
            enemy.SpecController.Target.Value = player;
            enemy.BehaviourController.Begin(enemySpec.Behaviour).Forget();
            gameCameraController.SetTrackingTarget(player.transform, enemy.transform);
            damageLabel.BeginObserve(enemy);
            var questClearContainer = new Container();
            questClearContainer.Register(this);
            questClearContainer.Register("Player", player);
            questClearContainer.Register("Enemy", enemy);
            var questClearSequencer = new Sequencer(questClearContainer, questSpec.QuestClearSequences.Sequences);
            questClearSequencer.PlayAsync(questScope.Token).Forget();
            var questFailedContainer = new Container();
            questFailedContainer.Register(this);
            questFailedContainer.Register("Player", player);
            questFailedContainer.Register("Enemy", enemy);
            var questFailedSequencer = new Sequencer(questFailedContainer, questSpec.QuestFailedSequences.Sequences);
            questFailedSequencer.PlayAsync(questScope.Token).Forget();
        }

        public void SetupHomeQuest()
        {
            SetupQuest(homeQuestSpecId);
        }

#if DEBUG
        public void SetupDefaultQuest()
        {
            SetupQuest(initialQuestSpecId);
        }
#endif
    }
}
