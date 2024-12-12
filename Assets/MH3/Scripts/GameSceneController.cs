using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;
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
        private int initialWeaponId;

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

        [SerializeField]
        private HKUIDocument fadeDocumentPrefab;

        [SerializeField]
        private HKUIDocument instanceWeaponViewDocumentPrefab;

        [SerializeField]
        private HKUIDocument instanceSkillCoreViewDocumentPrefab;

        [SerializeField]
        private HKUIDocument notificationCenterDocumentPrefab;

        [SerializeField]
        private HKUIDocument simpleDialogDocumentPrefab;

        private Actor player;

        private Actor enemy;

        private Stage stage;

        private GameCameraController gameCameraController;

        private UIViewDamageLabel damageLabel;

        private CancellationDisposable questScope;

        private MasterData.QuestSpec currentQuestSpec;

        private UIViewFade fade;

        private void Start()
        {
            var inputController = new InputController();
            TinyServiceLocator.RegisterAsync(inputController, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(audioManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(effectManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(new UIViewNotificationCenter(notificationCenterDocumentPrefab, destroyCancellationToken), destroyCancellationToken).Forget();
            gameCameraController = Instantiate(gameCameraControllerPrefab);
            TinyServiceLocator.RegisterAsync(gameCameraController, destroyCancellationToken).Forget();
            var userData = new UserData();
            TinyServiceLocator.RegisterAsync(userData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(new UIViewSimpleDialog(simpleDialogDocumentPrefab), destroyCancellationToken).Forget();
            var playerSpec = masterData.ActorSpecs.Get(playerActorSpecId);
            player = playerSpec.Spawn(Vector3.zero, Quaternion.identity);
            player.BehaviourController.Begin(playerSpec.Behaviour).Forget();
            var initialInstanceWeapon = InstanceWeaponFactory.Create(userData, initialWeaponId);
            userData.AddInstanceWeaponData(initialInstanceWeapon);
            player.SpecController.ChangeInstanceWeapon(initialInstanceWeapon, userData.InstanceSkillCoreList);
            _ = new UIViewPlayerStatus(playerStatusDocumentPrefab, player, destroyCancellationToken);
            damageLabel = new UIViewDamageLabel(damageLabelDocumentPrefab, gameCameraController.ControlledCamera, destroyCancellationToken);
            fade = new UIViewFade(fadeDocumentPrefab, destroyCancellationToken);
            SetupQuestAsync(initialQuestSpecId, immediate: true).Forget();
            inputController.Actions.Player.PauseMenu
                .OnPerformedAsObservable()
                .Subscribe(_ =>
                {
                    UIViewPauseMenu.OpenAsync(
                        headerDocumentPrefab,
                        listDocumentPrefab,
                        instanceWeaponViewDocumentPrefab,
                        instanceSkillCoreViewDocumentPrefab,
                        player,
                        this,
                        currentQuestSpec.Id == homeQuestSpecId,
                        destroyCancellationToken
                    ).Forget();
                })
                .RegisterTo(destroyCancellationToken);
#if DEBUG
            var debugData = new GameDebugData();
            var isOpenDebugMenu = false;
            TinyServiceLocator.RegisterAsync(debugData, destroyCancellationToken).Forget();
            inputController.Actions.Player.DebugMenu
                .OnPerformedAsObservable()
                .Subscribe(async _ =>
                {
                    if (!isOpenDebugMenu)
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

        public async UniTask SetupQuestAsync(string questSpecId, bool immediate = false)
        {
            if (questScope != null)
            {
                questScope.Dispose();
            }

            if (!immediate)
            {
                await fade.BeginAnimation("In");
            }

            enemy.DestroySafe();
            stage.DestroySafe();
            questScope = new CancellationDisposable();
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            currentQuestSpec = questSpec;
            stage = UnityEngine.Object.Instantiate(questSpec.StagePrefab);
            var enemySpec = TinyServiceLocator.Resolve<MasterData>().ActorSpecs.Get(questSpec.EnemyActorSpecId);
            enemy = enemySpec.Spawn(stage.EnemySpawnPoint.position, stage.EnemySpawnPoint.rotation);
            player.SpecController.ResetAll();
            player.transform.position = stage.PlayerSpawnPoint.position;
            player.MovementController.RotateImmediate(stage.PlayerSpawnPoint.rotation);
            player.SpecController.Target.Value = enemy;
            enemy.SpecController.Target.Value = player;
            enemy.BehaviourController.Begin(enemySpec.Behaviour).Forget();
            gameCameraController.Setup(player, enemy);
            damageLabel.BeginObserve(enemy);
            var questClearContainer = new Container();
            questClearContainer.Register(this);
            questClearContainer.Register("Player", player);
            questClearContainer.Register("Enemy", enemy);
            questClearContainer.Register("QuestSpecId", questSpecId);
            var questClearSequencer = new Sequencer(questClearContainer, questSpec.QuestClearSequences.Sequences);
            questClearSequencer.PlayAsync(questScope.Token).Forget();
            var questFailedContainer = new Container();
            questFailedContainer.Register(this);
            questFailedContainer.Register("Player", player);
            questFailedContainer.Register("Enemy", enemy);
            var questFailedSequencer = new Sequencer(questFailedContainer, questSpec.QuestFailedSequences.Sequences);
            questFailedSequencer.PlayAsync(questScope.Token).Forget();

            if (!immediate)
            {
                await fade.BeginAnimation("Out");
            }
        }

        public UniTask SetupHomeQuestAsync()
        {
            return SetupQuestAsync(homeQuestSpecId);
        }

#if DEBUG
        public UniTask SetupDefaultQuestAsync()
        {
            return SetupQuestAsync(initialQuestSpecId);
        }
#endif
    }
}
