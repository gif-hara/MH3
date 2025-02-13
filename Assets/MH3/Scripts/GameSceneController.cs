using System;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
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
        private MaterialManager materialManagerPrefab;

        [SerializeField]
        private GameGlobalVolumeController gameGlobalVolumeControllerPrefab;

        [SerializeField]
        private string playerName;

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
        private HKUIDocument instanceArmorViewDocumentPrefab;

        [SerializeField]
        private HKUIDocument instanceSkillCoreViewDocumentPrefab;

        [SerializeField]
        private HKUIDocument notificationCenterDocumentPrefab;

        [SerializeField]
        private HKUIDocument simpleDialogDocumentPrefab;

        [SerializeField]
        private HKUIDocument actorSpecStatusDocumentPrefab;

        [SerializeField]
        private HKUIDocument enemyStatusDocumentPrefab;

        [SerializeField]
        private HKUIDocument questSpecStatusDocumentPrefab;

        [SerializeField]
        private HKUIDocument transitionDocumentPrefab;

        [SerializeField]
        private HKUIDocument optionsSoundsDocumentPrefab;

        [SerializeField]
        private HKUIDocument inputGuideDocumentPrefab;

        [SerializeField]
        private HKUIDocument titleDocumentPrefab;

        [SerializeField]
        private HKUIDocument termDescriptionDocumentPrefab;

        [SerializeField]
        private HKUIDocument tipsDocumentPrefab;

        [SerializeField]
        private HKUIDocument optionsKeyConfigDocumentPrefab;

        [SerializeField]
        private HKUIDocument actorEventNotificationDocumentPrefab;

        [SerializeField]
        private Material skyBoxMaterial;

        [SerializeField]
        private float skyBoxRotationSpeed;

        [SerializeField]
        private bool isSkipTitle;

        private Actor player;

        private Actor enemy;

        private Stage stage;

        private GameCameraController gameCameraController;

        private UIViewDamageLabel damageLabel;

        private CancellationDisposable questScope;

        private MasterData.QuestSpec currentQuestSpec;

        private UIViewFade fade;

        private UIViewEnemyStatus enemyStatus;

        private float beginQuestTime;

        public float ElapsedQuestTime { get; private set; }

        private bool isFirstSetupQuest = true;

        private bool isQuestTransitioning;

        private void Start()
        {
#if DEBUG
            if (isSkipTitle)
            {
                isFirstSetupQuest = false;
            }
#endif
            HK.Time.Root.timeScale = 1.0f;
            TinyServiceLocator.RegisterAsync(new GameEvents(), destroyCancellationToken).Forget();
            var inputController = new InputController();
            TinyServiceLocator.RegisterAsync(inputController, destroyCancellationToken).Forget();
            if (!TinyServiceLocator.Contains<MasterData>())
            {
                TinyServiceLocator.RegisterAsync(masterData, destroyCancellationToken).Forget();
            }
            TinyServiceLocator.RegisterAsync(gameRules, destroyCancellationToken).Forget();
            var audioManager = Instantiate(audioManagerPrefab);
            TinyServiceLocator.RegisterAsync(audioManager, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(effectManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(Instantiate(materialManagerPrefab), destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(new UIViewNotificationCenter(notificationCenterDocumentPrefab, destroyCancellationToken), destroyCancellationToken).Forget();
            gameCameraController = Instantiate(gameCameraControllerPrefab);
            TinyServiceLocator.RegisterAsync(gameCameraController, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(new InputScheme(destroyCancellationToken), destroyCancellationToken).Forget();
            var uiViewTransition = new UIViewTransition(transitionDocumentPrefab, destroyCancellationToken);
            TinyServiceLocator.RegisterAsync(uiViewTransition, destroyCancellationToken).Forget();
            var uiViewInputGuide = new UIViewInputGuide(inputGuideDocumentPrefab, destroyCancellationToken);
            TinyServiceLocator.RegisterAsync(uiViewInputGuide, destroyCancellationToken).Forget();
            var uiViewTips = new UIViewTips(tipsDocumentPrefab, destroyCancellationToken);
            TinyServiceLocator.RegisterAsync(uiViewTips, destroyCancellationToken).Forget();
            var gameGlobalVolumeController = Instantiate(gameGlobalVolumeControllerPrefab);
            var containsSaveData = SaveSystem.Contains(SaveData.Path);
            var saveData = containsSaveData ? SaveSystem.Load<SaveData>(SaveData.Path) : new SaveData();
            if (!containsSaveData)
            {
                saveData.SystemData.MasterVolume = 1.0f;
                saveData.SystemData.BgmVolume = 0.4f;
                saveData.SystemData.SfxVolume = 1.0f;
                SaveSystem.Save(saveData, SaveData.Path);
            }
            TinyServiceLocator.RegisterAsync(saveData, destroyCancellationToken).Forget();
            audioManager.SetVolumeMaster(saveData.SystemData.MasterVolume);
            audioManager.SetVolumeBgm(saveData.SystemData.BgmVolume);
            audioManager.SetVolumeSfx(saveData.SystemData.SfxVolume);
            var userData = saveData.UserData;
            userData.InitializeIfNeed();
            if (!userData.AvailableContents.Contains(AvailableContents.Key.FirstPlay))
            {
                userData.AvailableContents.Add(AvailableContents.Key.FirstPlay);
                foreach (var i in gameRules.InitialWeaponIds)
                {
                    var instanceWeapon = InstanceWeaponFactory.Create(userData, i);
                    userData.AddInstanceWeaponData(instanceWeapon);
                }
                userData.EquippedInstanceWeaponId = userData.InstanceWeapons[0].InstanceId;
            }
            TinyServiceLocator.RegisterAsync(saveData.UserData, destroyCancellationToken).Forget();
            TinyServiceLocator.RegisterAsync(new UIViewSimpleDialog(simpleDialogDocumentPrefab), destroyCancellationToken).Forget();
            var playerSpec = masterData.ActorSpecs.Get(playerActorSpecId);
            player = playerSpec.Spawn(Vector3.zero, Quaternion.identity);
            gameGlobalVolumeController.BeginObserves(player);
            player.SpecController.ActorName = playerName;
            player.BehaviourController.Begin(playerSpec.Behaviour).Forget();
            player.SpecController.ChangeInstanceWeapon(userData.GetEquippedInstanceWeapon());
            uiViewInputGuide.Push(() =>
            {
                return player.SpecController.WeaponSpec.WeaponType switch
                {
                    Define.WeaponType.Sword => string.Format(
                        "{0}:移動 {1}:攻撃 {2}:回避 {3}:回復 {4}:ガード {5}:メニュー".Localized(),
                        InputSprite.GetTag(inputController.Actions.Player.Move),
                        InputSprite.GetTag(inputController.Actions.Player.Attack),
                        InputSprite.GetTag(inputController.Actions.Player.Dodge),
                        InputSprite.GetTag(inputController.Actions.Player.Recovery),
                        InputSprite.GetTag(inputController.Actions.Player.Guard),
                        InputSprite.GetTag(inputController.Actions.Player.PauseMenu)
                        ),
                    Define.WeaponType.DualSword => string.Format(
                        "{0}:移動 {1}:攻撃 {2}:回避 {3}:回復 {4}:研ぎ {5}:メニュー".Localized(),
                        InputSprite.GetTag(inputController.Actions.Player.Move),
                        InputSprite.GetTag(inputController.Actions.Player.Attack),
                        InputSprite.GetTag(inputController.Actions.Player.Dodge),
                        InputSprite.GetTag(inputController.Actions.Player.Recovery),
                        InputSprite.GetTag(inputController.Actions.Player.Guard),
                        InputSprite.GetTag(inputController.Actions.Player.PauseMenu)
                        ),
                    Define.WeaponType.Blade => string.Format(
                        "{0}:移動 {1}:攻撃 {2}:回避 {3}:回復 {4}:我慢 {5}:メニュー".Localized(),
                        InputSprite.GetTag(inputController.Actions.Player.Move),
                        InputSprite.GetTag(inputController.Actions.Player.Attack),
                        InputSprite.GetTag(inputController.Actions.Player.Dodge),
                        InputSprite.GetTag(inputController.Actions.Player.Recovery),
                        InputSprite.GetTag(inputController.Actions.Player.Guard),
                        InputSprite.GetTag(inputController.Actions.Player.PauseMenu)
                        ),
                    Define.WeaponType.Shield => string.Format(
                        "{0}:移動 {1}:攻撃 {2}:回避 {3}:回復 {4}:ガード {5}:メニュー".Localized(),
                        InputSprite.GetTag(inputController.Actions.Player.Move),
                        InputSprite.GetTag(inputController.Actions.Player.Attack),
                        InputSprite.GetTag(inputController.Actions.Player.Dodge),
                        InputSprite.GetTag(inputController.Actions.Player.Recovery),
                        InputSprite.GetTag(inputController.Actions.Player.Guard),
                        InputSprite.GetTag(inputController.Actions.Player.PauseMenu)
                        ),
                    Define.WeaponType.Spear => string.Format(
                        "{0}:移動 {1}:攻撃 {2}:旋回回避 {3}:回復 {4}:メニュー".Localized(),
                        InputSprite.GetTag(inputController.Actions.Player.Move),
                        InputSprite.GetTag(inputController.Actions.Player.Attack),
                        InputSprite.GetTag(inputController.Actions.Player.Dodge),
                        InputSprite.GetTag(inputController.Actions.Player.Recovery),
                        InputSprite.GetTag(inputController.Actions.Player.PauseMenu)
                        ),
                    _ => string.Empty,
                };
            }, destroyCancellationToken);
            player.SpecController.SetArmorId(Define.ArmorType.Head, userData.GetEquippedInstanceArmor(Define.ArmorType.Head)?.ArmorId ?? 0);
            player.SpecController.SetArmorId(Define.ArmorType.Arms, userData.GetEquippedInstanceArmor(Define.ArmorType.Arms)?.ArmorId ?? 0);
            player.SpecController.SetArmorId(Define.ArmorType.Body, userData.GetEquippedInstanceArmor(Define.ArmorType.Body)?.ArmorId ?? 0);
            player.SpecController.BuildStatuses();
            player.ActionController.OnBeginDualSwordDodgeMode
                .Subscribe(_ =>
                {
                    TinyServiceLocator.Resolve<UserData>().AvailableContents.Add(AvailableContents.Key.FirstInvokeSharpen);
                    masterData.AvailableContentsEvents.Get(Define.AvailableContentsEventTrigger.InvokeSharpen)
                        .PlayAsync(destroyCancellationToken)
                        .Forget();
                })
                .RegisterTo(destroyCancellationToken);
            player.ActionController.OnBeginBladeEnduranceMode
                .Subscribe(_ =>
                {
                    TinyServiceLocator.Resolve<UserData>().AvailableContents.Add(AvailableContents.Key.FirstInvokeEndurance);
                    masterData.AvailableContentsEvents.Get(Define.AvailableContentsEventTrigger.InvokeEndurance)
                        .PlayAsync(destroyCancellationToken)
                        .Forget();
                })
                .RegisterTo(destroyCancellationToken);
            player.ActionController.IsGuard
                .Where(x => x && player.SpecController.WeaponSpec.WeaponType == Define.WeaponType.Sword)
                .Subscribe(_ =>
                {
                    TinyServiceLocator.Resolve<UserData>().AvailableContents.Add(AvailableContents.Key.FirstGuardSword);
                    masterData.AvailableContentsEvents.Get(Define.AvailableContentsEventTrigger.InvokeGuardSword)
                        .PlayAsync(destroyCancellationToken)
                        .Forget();
                })
                .RegisterTo(destroyCancellationToken);
            _ = new UIViewPlayerStatus(playerStatusDocumentPrefab, player, destroyCancellationToken);
            damageLabel = new UIViewDamageLabel(damageLabelDocumentPrefab, gameCameraController.ControlledCamera, destroyCancellationToken);
            fade = new UIViewFade(fadeDocumentPrefab, destroyCancellationToken);
            UIViewActorEventNotification.Open(actorEventNotificationDocumentPrefab, player, destroyCancellationToken);
            TinyServiceLocator.RegisterAsync(fade, destroyCancellationToken).Forget();
            enemyStatus = new UIViewEnemyStatus(enemyStatusDocumentPrefab, destroyCancellationToken);
            SetupQuestAsync(initialQuestSpecId, immediate: true).Forget();
            inputController.Actions.Player.PauseMenu
                .OnPerformedAsObservable()
                .Where(_ => !isQuestTransitioning)
                .Subscribe(_ =>
                {
                    UIViewPauseMenu.OpenAsync(
                        headerDocumentPrefab,
                        listDocumentPrefab,
                        instanceWeaponViewDocumentPrefab,
                        instanceArmorViewDocumentPrefab,
                        instanceSkillCoreViewDocumentPrefab,
                        actorSpecStatusDocumentPrefab,
                        questSpecStatusDocumentPrefab,
                        optionsSoundsDocumentPrefab,
                        termDescriptionDocumentPrefab,
                        optionsKeyConfigDocumentPrefab,
                        player,
                        this,
                        currentQuestSpec.Id == homeQuestSpecId,
                        destroyCancellationToken
                    ).Forget();
                })
                .RegisterTo(destroyCancellationToken);
            var instanceSkyBoxMaterial = Instantiate(skyBoxMaterial);
            destroyCancellationToken.RegisterWithoutCaptureExecutionContext(() => Destroy(instanceSkyBoxMaterial));
            RenderSettings.skybox = instanceSkyBoxMaterial;
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    instanceSkyBoxMaterial.SetFloat("_Rotation", UnityEngine.Time.time * HK.Time.Root.timeScale * skyBoxRotationSpeed);
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
            isQuestTransitioning = true;
            if (questScope != null)
            {
                questScope.Dispose();
            }
            questScope = new CancellationDisposable();
            TinyServiceLocator.Resolve<GameEvents>().OnBeginQuestTransition.OnNext(Unit.Default);
            if (!immediate)
            {
                await TinyServiceLocator.Resolve<UIViewTransition>()
                    .Build()
                    .SetMaterial("Transition.2")
                    .BeginAsync(LMotion.Create(0.0f, 1.0f, 0.4f), questScope.Token);
            }
            TinyServiceLocator.Resolve<GameEvents>().OnTransitioned.OnNext(Unit.Default);

            enemy.DestroySafe();
            stage.DestroySafe();
            var questSpec = TinyServiceLocator.Resolve<MasterData>().QuestSpecs.Get(questSpecId);
            currentQuestSpec = questSpec;
            stage = UnityEngine.Object.Instantiate(questSpec.StagePrefab);
            var enemySpec = TinyServiceLocator.Resolve<MasterData>().ActorSpecs.Get(questSpec.EnemyActorSpecId);
            enemy = enemySpec.Spawn(stage.EnemySpawnPoint.position, stage.EnemySpawnPoint.rotation);
            player.SpecController.ResetAll();
            player.transform.position = stage.PlayerSpawnPoint.position;
            player.MovementController.RotateImmediate(stage.PlayerSpawnPoint.rotation);
            player.SpecController.Target.Value = enemy;
            gameCameraController.Setup(player, enemy);
            damageLabel.BeginObserve(enemy);
            enemyStatus.BeginObserve(enemy);
            TinyServiceLocator.Resolve<AudioManager>().PlayBgm(questSpec.BgmKey);
            // タイトル画面
            if (isFirstSetupQuest)
            {
                isFirstSetupQuest = false;
                await UIViewTitle.OpenAsync(titleDocumentPrefab, destroyCancellationToken);
            }
            var beginQuestContainer = new Container();
            beginQuestContainer.Register(this);
            beginQuestContainer.Register("Player", player);
            beginQuestContainer.Register("Enemy", enemy);
            var beginQuestSequencer = new Sequencer(beginQuestContainer, questSpec.BeginQuestSequences.Sequences);
            beginQuestSequencer.PlayAsync(questScope.Token).Forget();
            enemy.SpecController.Target.Value = player;
            enemy.BehaviourController.Begin(enemySpec.Behaviour).Forget();
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
            isQuestTransitioning = false;
            if (!immediate)
            {
                await TinyServiceLocator.Resolve<UIViewTransition>()
                    .Build()
                    .BeginAsync(LMotion.Create(1.0f, 0.0f, 0.4f), questScope.Token);
            }
        }

        public void BeginQuestTimer()
        {
            beginQuestTime = UnityEngine.Time.time;
        }

        public void EndQuestTimer()
        {
            ElapsedQuestTime = UnityEngine.Time.time - beginQuestTime;
        }

        public UniTask SetupHomeQuestAsync()
        {
            return SetupQuestAsync(homeQuestSpecId);
        }

        public UniTask SetupRetryQuestAsync()
        {
            return SetupQuestAsync(currentQuestSpec.Id);
        }

#if DEBUG
        public UniTask SetupDefaultQuestAsync()
        {
            return SetupQuestAsync(initialQuestSpecId);
        }
#endif
    }
}
