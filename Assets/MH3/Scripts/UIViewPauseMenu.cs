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
            HKUIDocument optionsDocument = null;
            List<UIViewTermDescription.Element> termDescriptionElements = null;
            Func<CancellationToken, UniTask> onEndTermDescriptionNextState = null;
            var listInitialIndexCaches = new Dictionary<string, int>();
            var gameEvents = TinyServiceLocator.Resolve<GameEvents>();
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
                    _ => UIViewTips.SetTip("敵スライム君と戦うクエストを選択します。".Localized())
                    );
                AddListElement(
                    "装備変更".Localized(),
                    _ => stateMachine.Change(StateChangeEquipmentTypeRoot),
                    _ => UIViewTips.SetTip("武器や防具を変更します。".Localized())
                );
                if (userData.AvailableContents.Contains(AvailableContents.Key.AcquireInstanceSkillCore))
                {
                    AddListElement(
                        "スキルコア装着".Localized(),
                        _ => stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon),
                        _ => UIViewTips.SetTip("武器にスキルコアを装着します。".Localized())
                    );
                }
                AddListElement(
                    "装備削除".Localized(),
                    _ => stateMachine.Change(StateRemoveEquipment),
                    _ => UIViewTips.SetTip("不要な武器や防具を削除します。".Localized())
                );
                if (userData.AvailableContents.Contains(AvailableContents.Key.AcquireInstanceSkillCore))
                {
                    AddListElement(
                        "スキルコア削除".Localized(),
                        _ => stateMachine.Change(StateRemoveInstanceSkillCore),
                        _ => UIViewTips.SetTip("不要なスキルコアを削除します。".Localized())
                    );
                }
                AddListElement(
                    "オプション".Localized(),
                    _ => stateMachine.Change(StateOptionsRoot),
                    _ => UIViewTips.SetTip("ゲームの設定を変更します。".Localized())
                );
                AddListElement(
                    "閉じる".Localized(),
                    _ => pauseMenuScope.Dispose(),
                    _ => UIViewTips.SetTip("ポーズメニューを閉じます。".Localized())
                );
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    listElements,
                    listInitialIndexCaches.TryGetValue("HomeRoot", out var index) ? index : 0
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

                void AddListElement(string header, Action<Unit> onClick, Action<BaseEventData> onSelect)
                {
                    var index = listElements.Count;
                    listElements.Add(document =>
                    {
                        UIViewList.ApplyAsSimpleElement(
                            document,
                            header,
                            onClick,
                            x =>
                            {
                                listInitialIndexCaches["HomeRoot"] = index;
                                onSelect(x);
                            }
                        );
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
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                x.GetEnemyActorSpec().LocalizedName,
                                _ =>
                                {
                                    gameSceneController.SetupQuestAsync(x.Id).Forget();
                                    userData.AvailableContents.Add(AvailableContents.Key.FirstBattle);
                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.BeginQuest.1");
                                    pauseMenuScope.Dispose();
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("QuestSpec", x);
                                    questSpecStatusDocument.Q<SequencesMonoBehaviour>("Sequences")
                                        .PlayAsync(container, scope).Forget();
                                });
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
                SetHeaderText("装備変更".Localized());
                var listElements = new List<Action<HKUIDocument>>();
                AddListElement(
                    "武器".Localized(),
                    _ => stateMachine.Change(StateChangeInstanceWeapon),
                    _ => UIViewTips.SetTip("武器を変更します。".Localized())
                );
                AddListElement(
                    "頭防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Head;
                        stateMachine.Change(StateChangeInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("頭に装備する防具を変更します。".Localized())
                );
                AddListElement(
                    "腕防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Arms;
                        stateMachine.Change(StateChangeInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("腕に装備する防具を変更します。".Localized())
                );
                AddListElement(
                    "胴防具".Localized(),
                    _ =>
                    {
                        selectedArmorType = Define.ArmorType.Body;
                        stateMachine.Change(StateChangeInstanceArmor);
                    },
                    _ => UIViewTips.SetTip("胴に装備する防具を変更します。".Localized())
                );
                AddListElement(
                    "戻る".Localized(),
                    _ => stateMachine.Change(StateHomeRoot),
                    _ => UIViewTips.SetTip("前のメニューに戻ります。".Localized())
                );
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    listElements,
                    listInitialIndexCaches.TryGetValue("ChangeEquipmentTypeRoot", out var index) ? index : 0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateHomeRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                void AddListElement(string header, Action<Unit> onClick, Action<BaseEventData> onSelect)
                {
                    var index = listElements.Count;
                    listElements.Add(document =>
                    {
                        UIViewList.ApplyAsSimpleElement(
                            document,
                            header,
                            onClick,
                            x =>
                            {
                                listInitialIndexCaches["ChangeEquipmentTypeRoot"] = index;
                                onSelect(x);
                            }
                        );
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
                            var header = userData.EquippedInstanceWeaponId == x.InstanceId
                                ? $"[E] {x.WeaponSpec.LocalizedName}"
                                : x.WeaponSpec.LocalizedName;
                            UIViewList.ApplyAsSimpleElement(document, header,
                                _ =>
                                {
                                    if (userData.EquippedInstanceWeaponId == x.InstanceId)
                                    {
                                        TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("既に装備しています".Localized()).Forget();
                                        return;
                                    }
                                    userData.EquippedInstanceWeaponId = x.InstanceId;
                                    actor.SpecController.ChangeInstanceWeapon(x);
                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                    stateMachine.Change(StateHomeRoot);
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceWeapon", x);
                                    instanceWeaponSequences.PlayAsync(container, scope).Forget();
                                    selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                    var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs;
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
                                                termDescriptionElements.Add(new(termDescriptionSpecs.Get("Skill")));
                                                x.InstanceSkillCores
                                                    .SelectMany(y => y.Skills)
                                                    .Select(y => y.SkillType)
                                                    .Distinct()
                                                    .OrderBy(y => y)
                                                    .ToList()
                                                    .ForEach(y => termDescriptionElements.Add(new(y.GetTermDescriptionSpec())));
                                            }
                                            onEndTermDescriptionNextState = StateChangeInstanceWeapon;
                                            stateMachine.Change(StateTermDescription);
                                        })
                                        .RegisterTo(selectScope.Token);
                                },
                                _ =>
                                {
                                    selectScope?.Cancel();
                                    selectScope?.Dispose();
                                }
                                );
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
                            var header = userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId == x.InstanceId
                                ? $"[E] {x.ArmorSpec.LocalizedName}"
                                : x.ArmorSpec.LocalizedName;
                            UIViewList.ApplyAsSimpleElement(document, header, _ =>
                                {
                                    var instanceId = userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId ==
                                                     x.InstanceId ? 0 : x.InstanceId;
                                    var instanceArmor = userData.InstanceArmors.FirstOrDefault(y => y.InstanceId == instanceId);
                                    userData.SetEquippedInstanceArmor(selectedArmorType, instanceId);
                                    actor.SpecController.SetArmorId(selectedArmorType, instanceArmor?.ArmorSpec.Id ?? 0);
                                    actor.SpecController.BuildStatuses();
                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                    stateMachine.Change(StateChangeEquipmentTypeRoot);
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceArmor", x);
                                    instanceArmorSequences.PlayAsync(container, scope).Forget();
                                    selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                    var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs;
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
                                                termDescriptionElements.Add(new(termDescriptionSpecs.Get("Skill")));
                                                x.Skills
                                                    .Select(y => y.SkillType)
                                                    .Distinct()
                                                    .OrderBy(y => y)
                                                    .ToList()
                                                    .ForEach(y => termDescriptionElements.Add(new(y.GetTermDescriptionSpec())));
                                            }
                                            onEndTermDescriptionNextState = StateChangeInstanceArmor;
                                            stateMachine.Change(StateTermDescription);
                                        })
                                        .RegisterTo(selectScope.Token);
                                },
                                _ =>
                                {
                                    selectScope?.Cancel();
                                    selectScope?.Dispose();
                                }
                                );
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
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "武器".Localized(),
                                _ => stateMachine.Change(StateRemoveInstanceWeapon),
                                _ => UIViewTips.SetTip("所持している武器を削除します。".Localized())
                                );
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "頭防具".Localized(),
                                _ =>
                                {
                                    selectedArmorType = Define.ArmorType.Head;
                                    stateMachine.Change(StateRemoveInstanceArmor);
                                },
                                _ => UIViewTips.SetTip("所持している頭防具を削除します。".Localized())
                                );
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "腕防具".Localized(),
                                _ =>
                                {
                                    selectedArmorType = Define.ArmorType.Arms;
                                    stateMachine.Change(StateRemoveInstanceArmor);
                                },
                                _ => UIViewTips.SetTip("所持している腕防具を削除します。".Localized())
                                );
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "胴防具".Localized(),
                                _ =>
                                {
                                    selectedArmorType = Define.ArmorType.Body;
                                    stateMachine.Change(StateRemoveInstanceArmor);
                                },
                                _ => UIViewTips.SetTip("所持している胴防具を削除します。".Localized())
                                );
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "戻る".Localized(),
                                _ => stateMachine.Change(StateHomeRoot),
                                _ => UIViewTips.SetTip("前のメニューに戻ります。".Localized())
                                );
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateHomeRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
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
                    var instanceWeapons = userData.InstanceWeapons.Where(x => x.InstanceId != userData.EquippedInstanceWeaponId);
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        instanceWeapons
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                CancellationTokenSource selectScope = null;
                                UIViewList.ApplyAsSimpleElement(document, x.WeaponSpec.LocalizedName, async _ =>
                                {
                                    tempSelection = document.Q<Selectable>("Button");
                                    dialogScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
                                    var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>().OpenAsync(
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
                                        SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                        CreateList();
                                    }
                                    else
                                    {
                                        tempSelection.Select();
                                    }
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceWeapon", x);
                                    instanceWeaponSequences.PlayAsync(container, scope).Forget();
                                    selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                    var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs;
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
                                                termDescriptionElements.Add(new(termDescriptionSpecs.Get("Skill")));
                                                x.InstanceSkillCores
                                                    .SelectMany(y => y.Skills)
                                                    .Select(y => y.SkillType)
                                                    .Distinct()
                                                    .OrderBy(y => y)
                                                    .ToList()
                                                    .ForEach(y => termDescriptionElements.Add(new(y.GetTermDescriptionSpec())));
                                            }
                                            onEndTermDescriptionNextState = StateRemoveInstanceWeapon;
                                            stateMachine.Change(StateTermDescription);
                                        })
                                        .RegisterTo(selectScope.Token);
                                },
                                _ =>
                                {
                                    selectScope?.Cancel();
                                    selectScope?.Dispose();
                                }
                                );
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
                                var header = userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId == x.InstanceId
                                    ? $"[E] {x.ArmorSpec.LocalizedName}"
                                    : x.ArmorSpec.LocalizedName;
                                UIViewList.ApplyAsSimpleElement(document, header, async _ =>
                                {
                                    tempSelection = document.Q<Selectable>("Button");
                                    dialogScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
                                    var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>().OpenAsync(
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
                                        if (userData.GetEquippedInstanceArmor(armorSpec.ArmorType)?.InstanceId ==
                                            x.InstanceId)
                                        {
                                            userData.SetEquippedInstanceArmor(armorSpec.ArmorType, 0);
                                            actor.SpecController.BuildStatuses();
                                        }
                                        userData.RemoveInstanceArmor(x);
                                        SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                        CreateList();
                                    }
                                    else
                                    {
                                        tempSelection.Select();
                                    }
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceArmor", x);
                                    instanceArmorSequences.PlayAsync(container, scope).Forget();
                                    selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                    var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs;
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
                                                termDescriptionElements.Add(new(termDescriptionSpecs.Get("Skill")));
                                                x.Skills
                                                    .Select(y => y.SkillType)
                                                    .Distinct()
                                                    .OrderBy(y => y)
                                                    .ToList()
                                                    .ForEach(y => termDescriptionElements.Add(new(y.GetTermDescriptionSpec())));
                                            }
                                            onEndTermDescriptionNextState = StateRemoveInstanceArmor;
                                            stateMachine.Change(StateTermDescription);
                                        })
                                        .RegisterTo(selectScope.Token);
                                },
                                _ =>
                                {
                                    selectScope?.Cancel();
                                    selectScope?.Dispose();
                                }
                                );
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
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            CancellationTokenSource selectScope = null;
                            var header = userData.EquippedInstanceWeaponId == x.InstanceId
                                ? $"[E] {x.WeaponSpec.LocalizedName}"
                                : x.WeaponSpec.LocalizedName;
                            UIViewList.ApplyAsSimpleElement(document, header, _ =>
                            {
                                if (x.SkillSlot <= 0)
                                {
                                    TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("スキルスロットがありません".Localized()).Forget();
                                    return;
                                }
                                selectedInstanceWeapon = x;
                                stateMachine.Change(StateAddInstanceSkillCoreSelectSkillCore);
                            },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceWeapon", x);
                                    instanceWeaponSequences.PlayAsync(container, scope).Forget();
                                    selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                                    var termDescriptionSpecs = TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs;
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
                                                termDescriptionElements.Add(new(termDescriptionSpecs.Get("Skill")));
                                                x.InstanceSkillCores
                                                    .SelectMany(y => y.Skills)
                                                    .Select(y => y.SkillType)
                                                    .Distinct()
                                                    .OrderBy(y => y)
                                                    .ToList()
                                                    .ForEach(y => termDescriptionElements.Add(new(y.GetTermDescriptionSpec())));
                                            }
                                            onEndTermDescriptionNextState = StateAddInstanceSkillCoreSelectInstanceWeapon;
                                            stateMachine.Change(StateTermDescription);
                                        })
                                        .RegisterTo(selectScope.Token);
                                },
                                _ =>
                                {
                                    selectScope?.Cancel();
                                    selectScope?.Dispose();
                                }
                                );
                        })),
                    0
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
                                    .EditButton(button =>
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
                                                selectScope?.Cancel();
                                                selectScope?.Dispose();
                                            })
                                            .RegisterTo(button.destroyCancellationToken);
                                    });
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
                UIViewTips.SetTip("装着するスキルコアを選択してください。".Localized());
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    instanceSkillCores
                        .OrderBy(x => selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId) ? 0 : 1)
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            CancellationTokenSource selectScope = null;
                            document.CreateListElementBuilder()
                                .EditHeader(header =>
                                    {
                                        header.text = selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId)
                                            ? $"[E] {x.SkillCoreSpec.LocalizedName}"
                                            : x.SkillCoreSpec.LocalizedName;
                                    }
                                )
                                .EditButton(button =>
                                {
                                    button.OnClickAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            var userData = TinyServiceLocator.Resolve<UserData>();
                                            if (selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId))
                                            {
                                                selectedInstanceWeapon.RemoveInstanceSkillCoreId(x.InstanceId);
                                            }
                                            else
                                            {
                                                if (selectedInstanceWeapon.GetUsingSlotCount(
                                                        userData.InstanceSkillCores) +
                                                    x.Slot > selectedInstanceWeapon.SkillSlot)
                                                {
                                                    TinyServiceLocator.Resolve<UIViewNotificationCenter>()
                                                        .BeginOneShotAsync("スキルスロットが足りません".Localized()).Forget();
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
                                            stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon);
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                    button.OnSelectAsObservable()
                                        .Subscribe(_ =>
                                        {
                                            var container = new Container();
                                            container.Register("InstanceSkillCore", x);
                                            instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                                            selectScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
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
                                            selectScope?.Cancel();
                                            selectScope?.Dispose();
                                        })
                                        .RegisterTo(button.destroyCancellationToken);
                                });
                        })),
                    0
                );
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
                            UIViewList.ApplyAsSimpleElement(document, "サウンド".Localized(), _ =>
                            {
                                optionsListSelection = document.Q<Selectable>("Button");
                                stateMachine.Change(StateOptionsSounds);
                            },
                            _ =>
                            {
                                UIViewTips.SetTip("マスター、BGM、効果音の音量設定を変更します。".Localized());
                                CreateOptionsDocument(optionsSoundsDocumentPrefab);
                                var saveData = TinyServiceLocator.Resolve<SaveData>();
                                optionsDocument
                                    .Q<HKUIDocument>("MasterVolume")
                                    .Q<HKUIDocument>("Area.Slider")
                                    .Q<HKUIDocument>("Element.Slider")
                                    .Q<Slider>("Slider")
                                    .value = saveData.SystemData.MasterVolume;
                                optionsDocument
                                    .Q<HKUIDocument>("BgmVolume")
                                    .Q<HKUIDocument>("Area.Slider")
                                    .Q<HKUIDocument>("Element.Slider")
                                    .Q<Slider>("Slider")
                                    .value = saveData.SystemData.BgmVolume;
                                optionsDocument
                                    .Q<HKUIDocument>("SfxVolume")
                                    .Q<HKUIDocument>("Area.Slider")
                                    .Q<HKUIDocument>("Element.Slider")
                                    .Q<Slider>("Slider")
                                    .value = saveData.SystemData.SfxVolume;
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "戻る".Localized(),
                                _ =>
                                {
                                    optionsListDocument.DestroySafe();
                                    stateMachine.Change(StateHomeRoot);
                                },
                                _ =>
                                {
                                    UIViewTips.SetTip("前のメニューに戻ります。".Localized());
                                    optionsDocument.DestroySafe();
                                }
                            );
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
                        optionsDocument.DestroySafe();
                        optionsListDocument.DestroySafe();
                        stateMachine.Change(StateHomeRoot);
                    })
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
            }

            async UniTask StateOptionsSounds(CancellationToken scope)
            {
                SetHeaderText("サウンド設定".Localized());
                var saveData = TinyServiceLocator.Resolve<SaveData>();
                var audioManager = TinyServiceLocator.Resolve<AudioManager>();
                var masterVolumeDocument = optionsDocument.Q<HKUIDocument>("MasterVolume");
                var masterVolumeSelectable = masterVolumeDocument
                    .Q<HKUIDocument>("Area.Button")
                    .Q<Selectable>("Button");
                var masterVolumeSlider = masterVolumeDocument
                    .Q<HKUIDocument>("Area.Slider")
                    .Q<HKUIDocument>("Element.Slider")
                    .Q<Slider>("Slider");
                var bgmVolumeDocument = optionsDocument.Q<HKUIDocument>("BgmVolume");
                var bgmVolumeSelectable = bgmVolumeDocument
                    .Q<HKUIDocument>("Area.Button")
                    .Q<Selectable>("Button");
                var bgmVolumeSlider = bgmVolumeDocument
                    .Q<HKUIDocument>("Area.Slider")
                    .Q<HKUIDocument>("Element.Slider")
                    .Q<Slider>("Slider");
                var sfxVolumeDocument = optionsDocument.Q<HKUIDocument>("SfxVolume");
                var sfxVolumeSelectable = sfxVolumeDocument
                    .Q<HKUIDocument>("Area.Button")
                    .Q<Selectable>("Button");
                var sfxVolumeSlider = sfxVolumeDocument
                    .Q<HKUIDocument>("Area.Slider")
                    .Q<HKUIDocument>("Element.Slider")
                    .Q<Slider>("Slider");
                new[]
                {
                    masterVolumeSelectable,
                    bgmVolumeSelectable,
                    sfxVolumeSelectable,
                }.SetNavigationVertical();

                masterVolumeSelectable.OnSelectAsObservable()
                    .Subscribe(_ => UIViewTips.SetTip("マスター音量を変更します。".Localized()))
                    .AddTo(scope);

                bgmVolumeSelectable.OnSelectAsObservable()
                    .Subscribe(_ => UIViewTips.SetTip("BGMの音量を変更します。".Localized()))
                    .AddTo(scope);

                sfxVolumeSelectable.OnSelectAsObservable()
                    .Subscribe(_ => UIViewTips.SetTip("効果音の音量を変更します。".Localized()))
                    .AddTo(scope);

                masterVolumeSelectable.Select();
                masterVolumeSlider.value = saveData.SystemData.MasterVolume;
                bgmVolumeSlider.value = saveData.SystemData.BgmVolume;
                sfxVolumeSlider.value = saveData.SystemData.SfxVolume;

                inputController.Actions.UI.Navigate.OnPerformedAsObservable()
                    .Subscribe(context =>
                    {
                        var value = context.ReadValue<Vector2>();
                        if (value.x == 0)
                        {
                            return;
                        }
                        var addValue = value.x > 0 ? 0.1f : -0.1f;
                        switch (EventSystem.current.currentSelectedGameObject)
                        {
                            case var x when x == masterVolumeSelectable.gameObject:
                                saveData.SystemData.MasterVolume = Mathf.Clamp(saveData.SystemData.MasterVolume + addValue, 0, 1);
                                masterVolumeSlider.value = saveData.SystemData.MasterVolume;
                                audioManager.SetVolumeMaster(saveData.SystemData.MasterVolume);
                                SaveSystem.Save(saveData, SaveData.Path);
                                break;
                            case var x when x == bgmVolumeSelectable.gameObject:
                                saveData.SystemData.BgmVolume = Mathf.Clamp(saveData.SystemData.BgmVolume + addValue, 0, 1);
                                bgmVolumeSlider.value = saveData.SystemData.BgmVolume;
                                audioManager.SetVolumeBgm(saveData.SystemData.BgmVolume);
                                SaveSystem.Save(saveData, SaveData.Path);
                                break;
                            case var x when x == sfxVolumeSelectable.gameObject:
                                saveData.SystemData.SfxVolume = Mathf.Clamp(saveData.SystemData.SfxVolume + addValue, 0, 1);
                                sfxVolumeSlider.value = saveData.SystemData.SfxVolume;
                                audioManager.SetVolumeSfx(saveData.SystemData.SfxVolume);
                                SaveSystem.Save(saveData, SaveData.Path);
                                break;
                        }
                    })
                    .RegisterTo(scope);

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

            void CreateOptionsDocument(HKUIDocument optionsDocumentPrefab)
            {
                if (optionsDocument != null)
                {
                    optionsDocument.DestroySafe();
                }
                optionsDocument = UnityEngine.Object.Instantiate(optionsDocumentPrefab);
            }
        }
    }
}
