using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnitySequencerSystem;

namespace MH3
{
    public class UIViewPauseMenu
    {
        public static async UniTask OpenAsync(
            HKUIDocument headerDocumentPrefab,
            HKUIDocument listDocumentPrefab,
            HKUIDocument instanceWeaponViewDocumentPrefab,
            HKUIDocument instanceArmorViewDocumentPrefab,
            HKUIDocument instanceSkillCoreViewDocumentPrefab,
            HKUIDocument actorSpecStatusDocumentPrefab,
            HKUIDocument questSpecStatusDocumentPrefab,
            HKUIDocument optionsSoundsDocumentPrefab,
            HKUIDocument termDescriptionDocumentPrefab,
            HKUIDocument optionsKeyConfigDocumentPrefab,
            Actor actor,
            GameSceneController gameSceneController,
            bool isHome,
            CancellationToken scope
            )
        {
            var pauseMenuScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
            var header = UnityEngine.Object.Instantiate(headerDocumentPrefab);
            var stateMachine = new TinyStateMachine();
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var uiViewInputGuide = TinyServiceLocator.Resolve<UIViewInputGuide>();
            uiViewInputGuide.Push(() => string.Format(
                "{0}:選択 {1}:決定 {2}:キャンセル",
                InputSprite.GetTag(inputController.Actions.UI.Navigate),
                InputSprite.GetTag(inputController.Actions.UI.Submit),
                InputSprite.GetTag(inputController.Actions.UI.Cancel)
                ).Localized(), pauseMenuScope.Token);
            InstanceWeapon selectedInstanceWeapon = null;
            Define.ArmorType selectedArmorType = Define.ArmorType.Head;
            HKUIDocument optionsListDocument = null;
            Selectable optionsListSelection = null;
            List<UIViewTermDescription.Element> termDescriptionElements = null;
            Func<CancellationToken, UniTask> onEndTermDescriptionNextState = null;
            var listInitialIndexCaches = new Dictionary<string, int>();
            var gameEvents = TinyServiceLocator.Resolve<GameEvents>();
            IUIViewOptions options = null;
            gameEvents.OnBeginQuestTransition
                .Subscribe(pauseMenuScope, static (_, p) => p.Dispose())
                .RegisterTo(pauseMenuScope.Token);


            if (isHome)
            {
                stateMachine.Change(StateHomeRoot);
            }
            else
            {
                stateMachine.Change(StateQuestRoot);
            }

            gameEvents.OnBeginPauseMenu.OnNext(Unit.Default);

            // 待機
            {
                await UniTask.WaitUntilCanceled(pauseMenuScope.Token);
            }

            // 終了処理
            {
                header.DestroySafe();
                inputController.PopActionType();
                pauseMenuScope.Dispose();
                stateMachine.Dispose();
                TinyServiceLocator.Resolve<GameEvents>().OnEndPauseMenu.OnNext(Unit.Default);
                UIViewTips.Close();
            }

            async UniTask StateHomeRoot(CancellationToken scope)
            {
                SetHeaderText("ホームメニュー".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var actorSpecStatusDocument = UnityEngine.Object.Instantiate(actorSpecStatusDocumentPrefab);
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                var listElements = new List<Action<HKUIDocument>>();
                AddListElement(
                    "クエスト選択".Localized(),
                    _ => stateMachine.Change(StateSelectQuest),
                    _ => UIViewTips.SetTip("敵スライム君と戦うクエストを選択します。".Localized()),
                    TinyServiceLocator.Resolve<MasterData>().QuestSpecs.List
                        .Where(x => userData.AvailableContents.Contains(x.NeedAvailableContentKey))
                        .Any(x => userData.Stats.GetOrDefault(Stats.Key.GetDefeatEnemyCount(x.EnemyActorSpecId)) == 0)
                    );
                AddListElement(
                    "装備変更".Localized(),
                    _ => stateMachine.Change(StateChangeEquipmentTypeRoot),
                    _ => UIViewTips.SetTip("武器や防具を変更します。".Localized()),
                    userData.InstanceWeapons.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)))
                    || userData.InstanceArmors.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                if (userData.AvailableContents.Contains(AvailableContents.Key.AcquireInstanceSkillCore))
                {
                    AddListElement(
                        "スキルコア装着".Localized(),
                        _ => stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon),
                        _ => UIViewTips.SetTip("武器にスキルコアを装着します。".Localized()),
                        userData.InstanceSkillCores.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceSkillCore(x.InstanceId)))
                    );
                }
                AddListElement(
                    "装備削除".Localized(),
                    _ => stateMachine.Change(StateRemoveEquipment),
                    _ => UIViewTips.SetTip("不要な武器や防具を削除します。".Localized()),
                    userData.InstanceWeapons.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)))
                    || userData.InstanceArmors.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                if (userData.AvailableContents.Contains(AvailableContents.Key.AcquireInstanceSkillCore))
                {
                    AddListElement(
                        "スキルコア削除".Localized(),
                        _ => stateMachine.Change(StateRemoveInstanceSkillCore),
                        _ => UIViewTips.SetTip("不要なスキルコアを削除します。".Localized()),
                        userData.InstanceSkillCores.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceSkillCore(x.InstanceId)))
                    );
                }
                AddListElement(
                    "オプション".Localized(),
                    _ => stateMachine.Change(StateOptionsRoot),
                    _ => UIViewTips.SetTip("ゲームの設定を変更します。".Localized()),
                    false
                );
                AddListElement(
                    "閉じる".Localized(),
                    _ => pauseMenuScope.Dispose(),
                    _ => UIViewTips.SetTip("ポーズメニューを閉じます。".Localized()),
                    false
                );
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    listElements,
                    listInitialIndexCaches.TryGetValue(nameof(StateHomeRoot), out var index) ? index : 0
                );
                var container = new Container();
                container.Register("Actor", actor);
                container.Register("InstanceWeapon", userData.GetEquippedInstanceWeapon());
                container.Register("InstanceArmorHead", userData.GetEquippedInstanceArmor(Define.ArmorType.Head));
                container.Register("InstanceArmorArms", userData.GetEquippedInstanceArmor(Define.ArmorType.Arms));
                container.Register("InstanceArmorBody", userData.GetEquippedInstanceArmor(Define.ArmorType.Body));
                var sequences = actorSpecStatusDocument.Q<SequencesMonoBehaviour>("Sequences");
                sequences.PlayAsync(container, scope).Forget();

                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => pauseMenuScope.Dispose())
                    .RegisterTo(scope);
                inputController.Actions.UI.Description
                    .OnPerformedAsObservable()
                    .Subscribe(_ =>
                    {
                        termDescriptionElements = new List<UIViewTermDescription.Element>
                        {
                            new(TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs.Get("Parameter")),
                        };
                        if (actor.SpecController.Skills.Count > 0)
                        {
                            termDescriptionElements.Add(new(TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs.Get("Skill")));
                            actor.SpecController.Skills
                                .Select(x => x.SkillType)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToList()
                                .ForEach(x => termDescriptionElements.Add(new(x.GetTermDescriptionSpec())));
                        }
                        onEndTermDescriptionNextState = StateHomeRoot;
                        stateMachine.Change(StateTermDescription);
                    })
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                actorSpecStatusDocument.DestroySafe();

                void AddListElement(string headerText, Action<Unit> onClick, Action<BaseEventData> onSelect, bool isActiveBadge)
                {
                    var index = listElements.Count;
                    listElements.Add(document =>
                    {
                        document.CreateListElementBuilder()
                            .EditHeader(header =>
                            {
                                header.text = headerText;
                            })
                            .EditButton(button =>
                            {
                                button.OnClickAsObservable()
                                    .Subscribe(onClick)
                                    .RegisterTo(button.destroyCancellationToken);
                                button.OnSelectAsObservable()
                                    .Subscribe(x =>
                                    {
                                        listInitialIndexCaches[nameof(StateHomeRoot)] = index;
                                        onSelect(x);
                                    })
                                    .RegisterTo(button.destroyCancellationToken);
                            })
                            .SetActiveBadge(isActiveBadge);
                    });
                }
            }

            async UniTask StateQuestRoot(CancellationToken scope)
            {
                SetHeaderText("クエストメニュー".Localized());
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "ホームに戻る".Localized(),
                                _ =>
                                {
                                    gameSceneController.SetupHomeQuestAsync().Forget();
                                    pauseMenuScope.Dispose();
                                },
                                _ => UIViewTips.SetTip("クエストをあきらめてホームに戻ります。".Localized())
                            );
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "リトライ".Localized(),
                                _ =>
                                {
                                    gameSceneController.SetupRetryQuestAsync().Forget();
                                    pauseMenuScope.Dispose();
                                },
                                _ => UIViewTips.SetTip("クエストをあきらめて再度同じスライム君と戦います。".Localized())
                            );
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "閉じる".Localized(),
                                _ => pauseMenuScope.Dispose(),
                                _ => UIViewTips.SetTip("ポーズメニューを閉じます。".Localized())
                                );
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => pauseMenuScope.Dispose())
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
            }

            async UniTask StateSelectQuest(CancellationToken scope)
            {
                var userData = TinyServiceLocator.Resolve<UserData>();
                var questSpecStatusDocument = UnityEngine.Object.Instantiate(questSpecStatusDocumentPrefab);
                UIViewTips.SetTip("戦いたい敵スライム君を選んでください。".Localized());
                SetHeaderText("クエスト選択".Localized());
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().QuestSpecs.List
                        .Where(x => userData.AvailableContents.Contains(x.NeedAvailableContentKey))
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            document.CreateListElementBuilder()
                                .EditHeader(header =>
                                {
                                    header.text = x.GetEnemyActorSpec().LocalizedName;
                                })
                                .EditButton(button =>
                                {
                                    button.OnClickAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            gameSceneController.SetupQuestAsync(x.Id).Forget();
                                            userData.AvailableContents.Add(AvailableContents.Key.FirstBattle);
                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                            TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.BeginQuest.1");
                                            pauseMenuScope.Dispose();
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnSelectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            var container = new Container();
                                            container.Register("QuestSpec", x);
                                            questSpecStatusDocument.Q<SequencesMonoBehaviour>("Sequences")
                                                .PlayAsync(container, scope).Forget();
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                })
                                .SetActiveBadge(userData.Stats.GetOrDefault(Stats.Key.GetDefeatEnemyCount(x.EnemyActorSpecId)) == 0);
                        })),
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateHomeRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                questSpecStatusDocument.DestroySafe();
            }

            async UniTask StateChangeEquipmentTypeRoot(CancellationToken scope)
            {
                var userData = TinyServiceLocator.Resolve<UserData>();
                SetHeaderText("装備変更".Localized());
                var listElements = new List<Action<HKUIDocument>>();
                AddListElement(
                    "武器".Localized(),
                    _ => stateMachine.Change(StateChangeInstanceWeapon),
                    _ => UIViewTips.SetTip("武器を変更します。".Localized()),
                    userData.InstanceWeapons.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)))
                );
                AddListElement(
                    "頭防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Head;
                        stateMachine.Change(StateChangeInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("頭に装備する防具を変更します。".Localized()),
                    userData.InstanceArmors.Any(x => x.ArmorSpec.ArmorType == Define.ArmorType.Head && !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                AddListElement(
                    "腕防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Arms;
                        stateMachine.Change(StateChangeInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("腕に装備する防具を変更します。".Localized()),
                    userData.InstanceArmors.Any(x => x.ArmorSpec.ArmorType == Define.ArmorType.Arms && !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                AddListElement(
                    "胴防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Body;
                        stateMachine.Change(StateChangeInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("胴に装備する防具を変更します。".Localized()),
                    userData.InstanceArmors.Any(x => x.ArmorSpec.ArmorType == Define.ArmorType.Body && !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                AddListElement(
                    "戻る".Localized(),
                    _ => stateMachine.Change(StateHomeRoot),
                    _ => UIViewTips.SetTip("前のメニューに戻ります。".Localized()),
                    false
                );
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    listElements,
                    listInitialIndexCaches.TryGetValue(nameof(StateChangeEquipmentTypeRoot), out var index) ? index : 0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateHomeRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                void AddListElement(string headerText, Action<Unit> onClick, Action<BaseEventData> onSelect, bool isActiveBadge)
                {
                    var index = listElements.Count;
                    listElements.Add(document =>
                    {
                        document.CreateListElementBuilder()
                            .EditHeader(header =>
                            {
                                header.text = headerText;
                            })
                            .EditButton(button =>
                            {
                                button.OnClickAsObservable()
                                    .Subscribe(onClick)
                                    .RegisterTo(button.destroyCancellationToken);
                                button.OnSelectAsObservable()
                                    .Subscribe(x =>
                                    {
                                        listInitialIndexCaches[nameof(StateChangeEquipmentTypeRoot)] = index;
                                        onSelect(x);
                                    })
                                    .RegisterTo(button.destroyCancellationToken);
                            })
                            .SetActiveBadge(isActiveBadge);
                    });
                }
            }

            async UniTask StateChangeInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("武器変更".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceWeapons = userData.InstanceWeapons;
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                UIViewTips.SetTip("装備する武器を選択してください。".Localized());
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    instanceWeapons
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            CancellationTokenSource selectScope = null;
                            var isEquiped = userData.EquippedInstanceWeaponId == x.InstanceId;
                            document.CreateListElementBuilder()
                                .EditHeader(header =>
                                {
                                    header.text = isEquiped
                                        ? $"[E] {x.WeaponSpec.LocalizedName}"
                                        : x.WeaponSpec.LocalizedName;
                                })
                                .EditButton((button, builder) =>
                                {
                                    button.OnClickAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            if (userData.EquippedInstanceWeaponId == x.InstanceId)
                                            {
                                                TinyServiceLocator.Resolve<UIViewNotificationCenter>()
                                                    .BeginOneShotAsync("既に装備しています".Localized()).Forget();
                                                return;
                                            }

                                            userData.EquippedInstanceWeaponId = x.InstanceId;
                                            actor.SpecController.ChangeInstanceWeapon(x);
                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                            TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                            stateMachine.Change(StateHomeRoot);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnSelectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId));
                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                            var container = new Container();
                                            container.Register("InstanceWeapon", x);
                                            instanceWeaponSequences.PlayAsync(container, scope).Forget();
                                            selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                            var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>()
                                                .TermDescriptionSpecs;
                                            inputController.Actions.UI.Description
                                                .OnPerformedAsObservable()
                                                .Subscribe(_ =>
                                                {
                                                    termDescriptionElements = new List<UIViewTermDescription.Element>
                                                    {
                                                        new(x.WeaponSpec.WeaponType.GetTermDescriptionSpec()),
                                                        new(termDescriptionSpecs.Get("Parameter")),
                                                    };
                                                    if (x.InstanceSkillCoreIds.Count > 0)
                                                    {
                                                        termDescriptionElements.Add(
                                                            new(termDescriptionSpecs.Get("Skill")));
                                                        x.InstanceSkillCores
                                                            .SelectMany(y => y.Skills)
                                                            .Select(y => y.SkillType)
                                                            .Distinct()
                                                            .OrderBy(y => y)
                                                            .ToList()
                                                            .ForEach(y =>
                                                                termDescriptionElements.Add(
                                                                    new(y.GetTermDescriptionSpec())));
                                                    }

                                                    onEndTermDescriptionNextState = StateChangeInstanceWeapon;
                                                    stateMachine.Change(StateTermDescription);
                                                })
                                                .RegisterTo(selectScope.Token);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnDeselectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            selectScope?.Cancel();
                                            selectScope?.Dispose();
                                            builder.SetActiveBadge(false);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                })
                                .ApplyStyle(isEquiped
                                    ? UIViewListElementBuilder.StyleNames.Primary
                                    : UIViewListElementBuilder.StyleNames.Default
                                    )
                                .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)));
                        })),
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateChangeEquipmentTypeRoot))
                    .RegisterTo(scope);
                instanceWeaponView.Q("Area.Default").SetActive(instanceWeapons.Count > 0);
                instanceWeaponView.Q("Area.Empty").SetActive(instanceWeapons.Count <= 0);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceWeaponView.DestroySafe();
            }

            async UniTask StateChangeInstanceArmor(CancellationToken scope)
            {
                SetHeaderText("防具変更".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceArmorView = UnityEngine.Object.Instantiate(instanceArmorViewDocumentPrefab);
                var instanceArmorSequences = instanceArmorView.Q<SequencesMonoBehaviour>("Sequences");
                var instanceArmors = userData.InstanceArmors
                    .Where(x => x.ArmorSpec.ArmorType == selectedArmorType);
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                UIViewTips.SetTip("装備する防具を選択してください。".Localized());
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    instanceArmors
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            CancellationTokenSource selectScope = null;
                            var isEquiped = userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId == x.InstanceId;
                            document.CreateListElementBuilder()
                                .EditHeader(header =>
                                {
                                    header.text = isEquiped
                                        ? $"[E] {x.ArmorSpec.LocalizedName}"
                                        : x.ArmorSpec.LocalizedName;
                                })
                                .EditButton((button, builder) =>
                                {
                                    button.OnClickAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            var instanceId =
                                                userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId ==
                                                x.InstanceId
                                                    ? 0
                                                    : x.InstanceId;
                                            var instanceArmor =
                                                userData.InstanceArmors.FirstOrDefault(y => y.InstanceId == instanceId);
                                            userData.SetEquippedInstanceArmor(selectedArmorType, instanceId);
                                            actor.SpecController.SetArmorId(selectedArmorType,
                                                instanceArmor?.ArmorSpec.Id ?? 0);
                                            actor.SpecController.BuildStatuses();
                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                            TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                            stateMachine.Change(StateChangeEquipmentTypeRoot);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnSelectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId));
                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                            var container = new Container();
                                            container.Register("InstanceArmor", x);
                                            instanceArmorSequences.PlayAsync(container, scope).Forget();
                                            selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                            var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>()
                                                .TermDescriptionSpecs;
                                            inputController.Actions.UI.Description
                                                .OnPerformedAsObservable()
                                                .Subscribe(_ =>
                                                {
                                                    termDescriptionElements = new List<UIViewTermDescription.Element>
                                                    {
                                                        new(termDescriptionSpecs.Get("Armor")),
                                                        new(termDescriptionSpecs.Get("Parameter")),
                                                    };
                                                    if (x.Skills.Count > 0)
                                                    {
                                                        termDescriptionElements.Add(
                                                            new(termDescriptionSpecs.Get("Skill")));
                                                        x.Skills
                                                            .Select(y => y.SkillType)
                                                            .Distinct()
                                                            .OrderBy(y => y)
                                                            .ToList()
                                                            .ForEach(y =>
                                                                termDescriptionElements.Add(
                                                                    new(y.GetTermDescriptionSpec())));
                                                    }

                                                    onEndTermDescriptionNextState = StateChangeInstanceArmor;
                                                    stateMachine.Change(StateTermDescription);
                                                })
                                                .RegisterTo(selectScope.Token);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnDeselectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            builder.SetActiveBadge(false);
                                            selectScope?.Cancel();
                                            selectScope?.Dispose();
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                })
                                .ApplyStyle(isEquiped
                                    ? UIViewListElementBuilder.StyleNames.Primary
                                    : UIViewListElementBuilder.StyleNames.Default
                                    )
                                .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)));
                        })),
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateChangeEquipmentTypeRoot))
                    .RegisterTo(scope);
                instanceArmorView.Q("Area.Default").SetActive(instanceArmors.Any());
                instanceArmorView.Q("Area.Empty").SetActive(!instanceArmors.Any());
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceArmorView.DestroySafe();
            }

            async UniTask StateRemoveEquipment(CancellationToken scope)
            {
                SetHeaderText("装備削除".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var listElements = new List<Action<HKUIDocument>>();
                AddListElement(
                    "武器".Localized(),
                    _ => stateMachine.Change(StateRemoveInstanceWeapon),
                    _ => UIViewTips.SetTip("所持している武器を削除します。".Localized()),
                    userData.InstanceWeapons.Any(x => !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)))
                    );
                AddListElement(
                    "頭防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Head;
                        stateMachine.Change(StateRemoveInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("所持している頭防具を削除します。".Localized()),
                    userData.InstanceArmors.Any(x => x.ArmorSpec.ArmorType == Define.ArmorType.Head && !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                AddListElement(
                    "腕防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Arms;
                        stateMachine.Change(StateRemoveInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("所持している腕防具を削除します。".Localized()),
                    userData.InstanceArmors.Any(x => x.ArmorSpec.ArmorType == Define.ArmorType.Arms && !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                AddListElement(
                    "胴防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Body;
                        stateMachine.Change(StateRemoveInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("所持している胴防具を削除します。".Localized()),
                    userData.InstanceArmors.Any(x => x.ArmorSpec.ArmorType == Define.ArmorType.Body && !userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)))
                );
                AddListElement(
                    "戻る".Localized(),
                    _ => stateMachine.Change(StateHomeRoot),
                    _ => UIViewTips.SetTip("前のメニューに戻ります。".Localized()),
                    false
                );
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    listElements,
                    listInitialIndexCaches.TryGetValue(nameof(StateRemoveEquipment), out var index) ? index : 0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateHomeRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                void AddListElement(string headerText, Action<Unit> onClick, Action<BaseEventData> onSelect, bool isActiveBadge)
                {
                    var index = listElements.Count;
                    listElements.Add(document =>
                    {
                        document.CreateListElementBuilder()
                            .EditHeader(header =>
                            {
                                header.text = headerText;
                            })
                            .EditButton(button =>
                            {
                                button.OnClickAsObservable()
                                    .Subscribe(onClick)
                                    .RegisterTo(button.destroyCancellationToken);
                                button.OnSelectAsObservable()
                                    .Subscribe(x =>
                                    {
                                        listInitialIndexCaches[nameof(StateRemoveEquipment)] = index;
                                        onSelect(x);
                                    })
                                    .RegisterTo(button.destroyCancellationToken);
                            })
                            .SetActiveBadge(isActiveBadge);
                    });
                }
            }

            async UniTask StateRemoveInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("武器削除".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                UIViewTips.SetTip("削除する武器を選択してください。".Localized());
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                CancellationDisposable dialogScope = null;
                Selectable tempSelection = null;
                HKUIDocument list = null;
                CreateList();
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Where(_ => dialogScope == null)
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateRemoveEquipment);
                    })
                    .RegisterTo(scope);
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    var instanceWeapons = userData.InstanceWeapons
                        .Where(x => x.InstanceId != userData.EquippedInstanceWeaponId);
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        instanceWeapons
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                CancellationTokenSource selectScope = null;
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                    {
                                        header.text = x.WeaponSpec.LocalizedName;
                                    })
                                    .EditButton((button, builder) =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(async _ =>
                                            {
                                                tempSelection = document.Q<Selectable>("Button");
                                                dialogScope =
                                                    new CancellationDisposable(
                                                        CancellationTokenSource.CreateLinkedTokenSource(scope));
                                                var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>()
                                                    .OpenAsync(
                                                        "本当に削除しますか？".Localized(),
                                                        new List<string> { "はい".Localized(), "いいえ".Localized() },
                                                        0,
                                                        1,
                                                        dialogScope.Token
                                                    );
                                                dialogScope.Dispose();
                                                dialogScope = null;
                                                if (result == 0)
                                                {
                                                    userData.RemoveInstanceWeapon(x);
                                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(),
                                                        SaveData.Path);
                                                    CreateList();
                                                }
                                                else
                                                {
                                                    tempSelection.Select();
                                                }
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId));
                                                SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                                var container = new Container();
                                                container.Register("InstanceWeapon", x);
                                                instanceWeaponSequences.PlayAsync(container, scope).Forget();
                                                selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                                var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>()
                                                    .TermDescriptionSpecs;
                                                inputController.Actions.UI.Description
                                                    .OnPerformedAsObservable()
                                                    .Subscribe(_ =>
                                                    {
                                                        termDescriptionElements =
                                                            new List<UIViewTermDescription.Element>
                                                            {
                                                                new(x.WeaponSpec.WeaponType.GetTermDescriptionSpec()),
                                                                new(termDescriptionSpecs.Get("Parameter")),
                                                            };
                                                        if (x.InstanceSkillCoreIds.Count > 0)
                                                        {
                                                            termDescriptionElements.Add(
                                                                new(termDescriptionSpecs.Get("Skill")));
                                                            x.InstanceSkillCores
                                                                .SelectMany(y => y.Skills)
                                                                .Select(y => y.SkillType)
                                                                .Distinct()
                                                                .OrderBy(y => y)
                                                                .ToList()
                                                                .ForEach(y =>
                                                                    termDescriptionElements.Add(
                                                                        new(y.GetTermDescriptionSpec())));
                                                        }

                                                        onEndTermDescriptionNextState = StateRemoveInstanceWeapon;
                                                        stateMachine.Change(StateTermDescription);
                                                    })
                                                    .RegisterTo(selectScope.Token);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnDeselectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                builder.SetActiveBadge(false);
                                                selectScope?.Cancel();
                                                selectScope?.Dispose();
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    })
                                    .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)));
                            })),
                        0
                    );
                    instanceWeaponView.Q("Area.Default").SetActive(instanceWeapons.Any());
                    instanceWeaponView.Q("Area.Empty").SetActive(!instanceWeapons.Any());
                }
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceWeaponView.DestroySafe();
            }

            async UniTask StateRemoveInstanceArmor(CancellationToken scope)
            {
                SetHeaderText("防具削除".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceArmorView = UnityEngine.Object.Instantiate(instanceArmorViewDocumentPrefab);
                var instanceArmorSequences = instanceArmorView.Q<SequencesMonoBehaviour>("Sequences");
                UIViewTips.SetTip("削除する防具を選択してください。".Localized());
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                CancellationDisposable dialogScope = null;
                Selectable tempSelection = null;
                HKUIDocument list = null;
                CreateList();
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Where(_ => dialogScope == null)
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateRemoveEquipment);
                    })
                    .RegisterTo(scope);
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    var instanceArmors = userData.InstanceArmors
                        .Where(x => x.ArmorSpec.ArmorType == selectedArmorType);
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        userData.InstanceArmors
                            .Where(x => userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId != x.InstanceId)
                            .Where(x => x.ArmorSpec.ArmorType == selectedArmorType)
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                CancellationTokenSource selectScope = null;
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                    {
                                        header.text = x.ArmorSpec.LocalizedName;
                                    })
                                    .EditButton((button, builder) =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(async _ =>
                                            {
                                                tempSelection = document.Q<Selectable>("Button");
                                                dialogScope =
                                                    new CancellationDisposable(
                                                        CancellationTokenSource.CreateLinkedTokenSource(scope));
                                                var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>()
                                                    .OpenAsync(
                                                        "本当に削除しますか？".Localized(),
                                                        new List<string> { "はい".Localized(), "いいえ".Localized() },
                                                        0,
                                                        1,
                                                        dialogScope.Token
                                                    );
                                                dialogScope.Dispose();
                                                dialogScope = null;
                                                if (result == 0)
                                                {
                                                    var armorSpec = x.ArmorSpec;
                                                    if (userData.GetEquippedInstanceArmor(armorSpec.ArmorType)
                                                            ?.InstanceId ==
                                                        x.InstanceId)
                                                    {
                                                        userData.SetEquippedInstanceArmor(armorSpec.ArmorType, 0);
                                                        actor.SpecController.BuildStatuses();
                                                    }

                                                    userData.RemoveInstanceArmor(x);
                                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(),
                                                        SaveData.Path);
                                                    CreateList();
                                                }
                                                else
                                                {
                                                    tempSelection.Select();
                                                }
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId));
                                                SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                                var container = new Container();
                                                container.Register("InstanceArmor", x);
                                                instanceArmorSequences.PlayAsync(container, scope).Forget();
                                                selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                                var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>()
                                                    .TermDescriptionSpecs;
                                                inputController.Actions.UI.Description
                                                    .OnPerformedAsObservable()
                                                    .Subscribe(_ =>
                                                    {
                                                        termDescriptionElements =
                                                            new List<UIViewTermDescription.Element>
                                                            {
                                                                new(termDescriptionSpecs.Get("Armor")),
                                                                new(termDescriptionSpecs.Get("Parameter")),
                                                            };
                                                        if (x.Skills.Count > 0)
                                                        {
                                                            termDescriptionElements.Add(
                                                                new(termDescriptionSpecs.Get("Skill")));
                                                            x.Skills
                                                                .Select(y => y.SkillType)
                                                                .Distinct()
                                                                .OrderBy(y => y)
                                                                .ToList()
                                                                .ForEach(y =>
                                                                    termDescriptionElements.Add(
                                                                        new(y.GetTermDescriptionSpec())));
                                                        }

                                                        onEndTermDescriptionNextState = StateRemoveInstanceArmor;
                                                        stateMachine.Change(StateTermDescription);
                                                    })
                                                    .RegisterTo(selectScope.Token);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnDeselectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                builder.SetActiveBadge(false);
                                                selectScope?.Cancel();
                                                selectScope?.Dispose();
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    })
                                    .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceArmor(x.InstanceId)));
                            })),
                        0
                    );
                    instanceArmorView.Q("Area.Default").SetActive(instanceArmors.Any());
                    instanceArmorView.Q("Area.Empty").SetActive(!instanceArmors.Any());
                }
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceArmorView.DestroySafe();
            }

            async UniTask StateAddInstanceSkillCoreSelectInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("スキルコア装着".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                var instanceWeapons = TinyServiceLocator.Resolve<UserData>().InstanceWeapons.Where(x => x.SkillSlot > 0);
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                UIViewTips.SetTip("スキルコアを装着する武器を選択してください。".Localized());
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    instanceWeapons
                        .Select((x, i) => new Action<HKUIDocument>(document =>
                        {
                            CancellationTokenSource selectScope = null;
                            var isEquiped = userData.EquippedInstanceWeaponId == x.InstanceId;
                            document.CreateListElementBuilder()
                                .EditHeader(header =>
                                {
                                    header.text = isEquiped
                                        ? $"[E] {x.WeaponSpec.LocalizedName}"
                                        : x.WeaponSpec.LocalizedName;
                                })
                                .EditButton((button, builder) =>
                                {
                                    button.OnClickAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            if (x.SkillSlot <= 0)
                                            {
                                                TinyServiceLocator.Resolve<UIViewNotificationCenter>()
                                                    .BeginOneShotAsync("スキルスロットがありません".Localized()).Forget();
                                                return;
                                            }

                                            selectedInstanceWeapon = x;
                                            stateMachine.Change(StateAddInstanceSkillCoreSelectSkillCore);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnSelectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId));
                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                            var container = new Container();
                                            container.Register("InstanceWeapon", x);
                                            instanceWeaponSequences.PlayAsync(container, scope).Forget();
                                            selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                            listInitialIndexCaches[nameof(StateAddInstanceSkillCoreSelectInstanceWeapon)] = i;
                                            var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>()
                                                .TermDescriptionSpecs;
                                            inputController.Actions.UI.Description
                                                .OnPerformedAsObservable()
                                                .Subscribe(_ =>
                                                {
                                                    termDescriptionElements = new List<UIViewTermDescription.Element>
                                                    {
                                                        new(x.WeaponSpec.WeaponType.GetTermDescriptionSpec()),
                                                        new(termDescriptionSpecs.Get("Parameter")),
                                                    };
                                                    if (x.InstanceSkillCoreIds.Count > 0)
                                                    {
                                                        termDescriptionElements.Add(
                                                            new(termDescriptionSpecs.Get("Skill")));
                                                        x.InstanceSkillCores
                                                            .SelectMany(y => y.Skills)
                                                            .Select(y => y.SkillType)
                                                            .Distinct()
                                                            .OrderBy(y => y)
                                                            .ToList()
                                                            .ForEach(y =>
                                                                termDescriptionElements.Add(
                                                                    new(y.GetTermDescriptionSpec())));
                                                    }

                                                    onEndTermDescriptionNextState =
                                                        StateAddInstanceSkillCoreSelectInstanceWeapon;
                                                    stateMachine.Change(StateTermDescription);
                                                })
                                                .RegisterTo(selectScope.Token);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnDeselectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            builder.SetActiveBadge(false);
                                            selectScope?.Cancel();
                                            selectScope?.Dispose();
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                })
                                .ApplyStyle(isEquiped
                                    ? UIViewListElementBuilder.StyleNames.Primary
                                    : UIViewListElementBuilder.StyleNames.Default
                                    )
                                .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceWeapon(x.InstanceId)));
                        })),
                    listInitialIndexCaches.TryGetValue(nameof(StateAddInstanceSkillCoreSelectInstanceWeapon), out var index) ? index : 0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateHomeRoot);
                    })
                    .RegisterTo(scope);
                instanceWeaponView.Q("Area.Default").SetActive(instanceWeapons.Any());
                instanceWeaponView.Q("Area.Empty").SetActive(!instanceWeapons.Any());
                await UniTask.WaitUntilCanceled(scope);
                instanceWeaponView.DestroySafe();
                list.DestroySafe();
            }

            async UniTask StateRemoveInstanceSkillCore(CancellationToken scope)
            {
                SetHeaderText("スキルコア削除".Localized());
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceSkillCoreView = UnityEngine.Object.Instantiate(instanceSkillCoreViewDocumentPrefab);
                var instanceSkillCoreSequences = instanceSkillCoreView.Q<SequencesMonoBehaviour>("Sequences");
                UIViewTips.SetTip("削除するスキルコアを選択してください。".Localized());
                uiViewInputGuide.Push(() => string.Format(
                    "{0}:選択 {1}:決定 {2}:キャンセル {3}:用語説明",
                    InputSprite.GetTag(inputController.Actions.UI.Navigate),
                    InputSprite.GetTag(inputController.Actions.UI.Submit),
                    InputSprite.GetTag(inputController.Actions.UI.Cancel),
                    InputSprite.GetTag(inputController.Actions.UI.Description)
                    ).Localized(), scope);
                CancellationDisposable dialogScope = null;
                HKUIDocument list = null;
                CreateList();
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    var instanceSkillCores = userData.InstanceSkillCores;
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        instanceSkillCores
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                CancellationTokenSource selectScope = null;
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                    {
                                        header.text = x.SkillCoreSpec.LocalizedName;
                                    })
                                    .EditButton((button, builder) =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(async _ =>
                                            {
                                                var tempSelection = document.Q<Button>("Button");
                                                dialogScope =
                                                    new CancellationDisposable(
                                                        CancellationTokenSource.CreateLinkedTokenSource(scope));
                                                var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>()
                                                    .OpenAsync(
                                                        "本当に削除しますか？".Localized(),
                                                        new List<string> { "はい".Localized(), "いいえ".Localized() },
                                                        0,
                                                        1,
                                                        dialogScope.Token
                                                    );
                                                dialogScope.Dispose();
                                                dialogScope = null;
                                                if (result == 0)
                                                {
                                                    if (userData.AnyAttachedSkillCore(x.InstanceId))
                                                    {
                                                        dialogScope =
                                                            new CancellationDisposable(CancellationTokenSource
                                                                .CreateLinkedTokenSource(scope));
                                                        result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>()
                                                            .OpenAsync(
                                                                "武器に装着されてますが、本当に削除しますか？削除する場合、自動的に外れます".Localized(),
                                                                new List<string>
                                                                    { "はい".Localized(), "いいえ".Localized() },
                                                                0,
                                                                1,
                                                                dialogScope.Token
                                                            );
                                                        dialogScope.Dispose();
                                                        dialogScope = null;
                                                        if (result == 0)
                                                        {
                                                            userData.RemoveInstanceSkillCore(x);
                                                            SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(),
                                                                SaveData.Path);
                                                            CreateList();
                                                        }
                                                        else
                                                        {
                                                            tempSelection.Select();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        userData.RemoveInstanceSkillCore(x);
                                                        SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(),
                                                            SaveData.Path);
                                                        CreateList();
                                                    }
                                                }
                                                else
                                                {
                                                    tempSelection.Select();
                                                }
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceSkillCore(x.InstanceId));
                                                SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                                var container = new Container();
                                                container.Register("InstanceSkillCore", x);
                                                instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                                                selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                                var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>()
                                                    .TermDescriptionSpecs;
                                                inputController.Actions.UI.Description
                                                    .OnPerformedAsObservable()
                                                    .Subscribe(_ =>
                                                    {
                                                        termDescriptionElements =
                                                            new List<UIViewTermDescription.Element>
                                                            {
                                                                new(termDescriptionSpecs.Get("SkillCore")),
                                                            };
                                                        if (x.Skills.Count > 0)
                                                        {
                                                            termDescriptionElements.Add(
                                                                new(termDescriptionSpecs.Get("Skill")));
                                                            x.Skills
                                                                .Select(y => y.SkillType)
                                                                .Distinct()
                                                                .OrderBy(y => y)
                                                                .ToList()
                                                                .ForEach(y =>
                                                                    termDescriptionElements.Add(
                                                                        new(y.GetTermDescriptionSpec())));
                                                        }

                                                        onEndTermDescriptionNextState = StateRemoveInstanceSkillCore;
                                                        stateMachine.Change(StateTermDescription);
                                                    })
                                                    .RegisterTo(selectScope.Token);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnDeselectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                builder.SetActiveBadge(false);
                                                selectScope?.Cancel();
                                                selectScope?.Dispose();
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    })
                                    .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceSkillCore(x.InstanceId)));
                            })),
                        0
                    );
                    instanceSkillCoreView.Q("Area.Default").SetActive(instanceSkillCores.Any());
                    instanceSkillCoreView.Q("Area.Empty").SetActive(!instanceSkillCores.Any());
                }
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Where(_ => dialogScope == null)
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateHomeRoot);
                    })
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                instanceSkillCoreView.DestroySafe();
                list.DestroySafe();
            }

            async UniTask StateAddInstanceSkillCoreSelectSkillCore(CancellationToken scope)
            {
                SetHeaderText("スキルコア装着".Localized());
                var instanceSkillCoreView = UnityEngine.Object.Instantiate(instanceSkillCoreViewDocumentPrefab);
                var instanceSkillCoreSequences = instanceSkillCoreView.Q<SequencesMonoBehaviour>("Sequences");
                var instanceSkillCores = TinyServiceLocator.Resolve<UserData>().InstanceSkillCores;
                var listIndex = 0;
                HKUIDocument list = null;
                UIViewTips.SetTip("装着するスキルコアを選択してください。".Localized());
                CreateList();
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        instanceSkillCores
                            .Select((x, i) => new Action<HKUIDocument>(document =>
                            {
                                CancellationTokenSource selectScope = null;
                                var userData = TinyServiceLocator.Resolve<UserData>();
                                var isEquiped = selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId);
                                var canAttach = selectedInstanceWeapon.GetUsingSlotCount(userData.InstanceSkillCores) + x.Slot
                                                <= selectedInstanceWeapon.SkillSlot;
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                        {
                                            header.text = isEquiped
                                                ? $"[E] {x.SkillCoreSpec.LocalizedName}"
                                                : x.SkillCoreSpec.LocalizedName;
                                        }
                                    )
                                    .EditButton((button, builder) =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                if (selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId))
                                                {
                                                    selectedInstanceWeapon.RemoveInstanceSkillCoreId(x.InstanceId);
                                                }
                                                else
                                                {
                                                    if (!canAttach)
                                                    {
                                                        TinyServiceLocator.Resolve<UIViewNotificationCenter>()
                                                            .BeginOneShotAsync("スキルスロットが足りないため装着出来ません".Localized()).Forget();
                                                        return;
                                                    }

                                                    selectedInstanceWeapon.AddInstanceSkillCoreId(x.InstanceId);
                                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                                }

                                                if (userData.EquippedInstanceWeaponId == selectedInstanceWeapon.InstanceId)
                                                {
                                                    actor.SpecController.ChangeInstanceWeapon(selectedInstanceWeapon);
                                                }

                                                SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                                CreateList();
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                userData.AvailableContents.Add(AvailableContents.Key.GetSeenInstanceSkillCore(x.InstanceId));
                                                SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                                var container = new Container();
                                                container.Register("InstanceSkillCore", x);
                                                instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                                                selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                                listIndex = i;
                                                var termDescriptionSpecs =
                                                    TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs;
                                                inputController.Actions.UI.Description
                                                    .OnPerformedAsObservable()
                                                    .Subscribe(_ =>
                                                    {
                                                        termDescriptionElements = new List<UIViewTermDescription.Element>
                                                        {
                                                        new(termDescriptionSpecs.Get("SkillCore")),
                                                        };
                                                        if (x.Skills.Count > 0)
                                                        {
                                                            termDescriptionElements.Add(
                                                                new(termDescriptionSpecs.Get("Skill")));
                                                            x.Skills
                                                                .Select(y => y.SkillType)
                                                                .Distinct()
                                                                .OrderBy(y => y)
                                                                .ToList()
                                                                .ForEach(y =>
                                                                    termDescriptionElements.Add(
                                                                        new(y.GetTermDescriptionSpec())));
                                                        }

                                                        onEndTermDescriptionNextState =
                                                            StateAddInstanceSkillCoreSelectSkillCore;
                                                        stateMachine.Change(StateTermDescription);
                                                    })
                                                    .RegisterTo(selectScope.Token);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnDeselectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                builder.SetActiveBadge(false);
                                                selectScope?.Cancel();
                                                selectScope?.Dispose();
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    })
                                    .ApplyStyle(isEquiped
                                        ? UIViewListElementBuilder.StyleNames.Primary
                                        : canAttach
                                            ? UIViewListElementBuilder.StyleNames.Default
                                            : UIViewListElementBuilder.StyleNames.Deactive
                                        )
                                    .SetActiveBadge(!userData.AvailableContents.Contains(AvailableContents.Key.GetSeenInstanceSkillCore(x.InstanceId)));
                            })),
                        listIndex
                    );
                }
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon);
                    })
                    .RegisterTo(scope);
                instanceSkillCoreView.Q("Area.Default").SetActive(instanceSkillCores.Any());
                instanceSkillCoreView.Q("Area.Empty").SetActive(!instanceSkillCores.Any());

                await UniTask.WaitUntilCanceled(scope);

                instanceSkillCoreView.DestroySafe();
                list.DestroySafe();
            }

            async UniTask StateOptionsRoot(CancellationToken scope)
            {
                SetHeaderText("オプション".Localized());
                if (optionsListDocument == null)
                {
                    optionsListDocument = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        new List<Action<HKUIDocument>>
                        {
                            document =>
                            {
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                    {
                                        header.text = "サウンド".Localized();
                                    })
                                    .EditButton(button =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                optionsListSelection = button;
                                                stateMachine.Change(StateOptionsSounds);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                UIViewTips.SetTip("マスター、BGM、効果音の音量設定を変更します。".Localized());
                                                RegisterUIViewOptions(new UIViewOptionsSound(optionsSoundsDocumentPrefab));
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    });
                            },
                            document =>
                            {
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                    {
                                        header.text = "キーコンフィグ".Localized();
                                    })
                                    .EditButton(button =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                optionsListSelection = button;
                                                stateMachine.Change(StateOptionsKeyConfig);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                UIViewTips.SetTip("操作するキーの設定を行います。".Localized());
                                                RegisterUIViewOptions(new UIViewOptionsKeyConfig(optionsKeyConfigDocumentPrefab));
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    });
                            },
                            document =>
                            {
                                document.CreateListElementBuilder()
                                    .EditHeader(header =>
                                    {
                                        header.text = "戻る".Localized();
                                    })
                                    .EditButton(button =>
                                    {
                                        button.OnClickAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                optionsListDocument.DestroySafe();
                                                stateMachine.Change(StateHomeRoot);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                        button.OnSelectAsObservable()
                                            .Subscribe(_ =>
                                            {
                                                UIViewTips.SetTip("前のメニューに戻ります。".Localized());
                                                RegisterUIViewOptions(null);
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    });
                            },
                        },
                        0
                    );
                }
                else
                {
                    optionsListSelection.Select();
                }
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ =>
                    {
                        RegisterUIViewOptions(null);
                        optionsListDocument.DestroySafe();
                        stateMachine.Change(StateHomeRoot);
                    })
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
            }

            async UniTask StateOptionsSounds(CancellationToken scope)
            {
                SetHeaderText("サウンド設定".Localized());
                await options.ActivateAsync(scope);
                stateMachine.Change(StateOptionsRoot);
            }

            async UniTask StateOptionsKeyConfig(CancellationToken scope)
            {
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateOptionsRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
            }

            async UniTask StateTermDescription(CancellationToken scope)
            {
                UIViewTips.Close();
                SetHeaderText("用語説明".Localized());
                await UIViewTermDescription.OpenAsync(
                    listDocumentPrefab,
                    termDescriptionDocumentPrefab,
                    termDescriptionElements,
                    scope
                );
                stateMachine.Change(onEndTermDescriptionNextState);
            }

            void SetHeaderText(string text)
            {
                header.Q<TMP_Text>("Header").text = text;
            }

            void RegisterUIViewOptions(IUIViewOptions newOptions)
            {
                if (options != null)
                {
                    options.Dispose();
                }

                options = newOptions;
            }
        }
    }
}
