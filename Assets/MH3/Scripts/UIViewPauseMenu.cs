using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using TMPro;
using UnityEngine;
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
            InstanceWeapon selectedInstanceWeapon = null;
            Define.ArmorType selectedArmorType = Define.ArmorType.Head;

            if (isHome)
            {
                stateMachine.Change(StateHomeRoot);
            }
            else
            {
                stateMachine.Change(StateQuestRoot);
            }

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
            }

            async UniTask StateHomeRoot(CancellationToken scope)
            {
                SetHeaderText("ホームメニュー");
                var actorSpecStatusDocument = UnityEngine.Object.Instantiate(actorSpecStatusDocumentPrefab);
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "クエスト選択", _ => stateMachine.Change(StateSelectQuest));
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "武器変更", _ =>
                            {
                                stateMachine.Change(StateChangeInstanceWeapon);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "防具変更", _ =>
                            {
                                stateMachine.Change(StateChangeInstanceArmorRoot);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "スキルコア装着", _ =>
                            {
                                stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "武器削除", _ =>
                            {
                                stateMachine.Change(StateRemoveInstanceWeapon);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "防具削除", _ =>
                            {
                                stateMachine.Change(StateRemoveInstanceArmor);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "スキルコア削除", _ =>
                            {
                                stateMachine.Change(StateRemoveInstanceSkillCore);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "閉じる", _ => pauseMenuScope.Dispose());
                        },
                    },
                    0
                );
                var userData = TinyServiceLocator.Resolve<UserData>();
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
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                actorSpecStatusDocument.DestroySafe();
            }

            async UniTask StateQuestRoot(CancellationToken scope)
            {
                SetHeaderText("クエストメニュー");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "ホームに戻る", _ =>
                            {
                                gameSceneController.SetupHomeQuestAsync();
                                pauseMenuScope.Dispose();
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "閉じる", _ => pauseMenuScope.Dispose());
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
                SetHeaderText("クエスト選択");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().QuestSpecs.List
                        .Where(x => userData.AvailableContents.Contains(x.NeedAvailableContentKey))
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                x.Id,
                                _ =>
                                {
                                    gameSceneController.SetupQuestAsync(x.Id).Forget();
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

            async UniTask StateChangeInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("武器変更");
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    userData.InstanceWeapons
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            var header = userData.EquippedInstanceWeaponId == x.InstanceId
                                ? $"[E] {x.WeaponSpec.Name}"
                                : x.WeaponSpec.Name;
                            UIViewList.ApplyAsSimpleElement(document, header, _ =>
                                {
                                    if (userData.EquippedInstanceWeaponId == x.InstanceId)
                                    {
                                        TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("既に装備しています").Forget();
                                        return;
                                    }
                                    userData.EquippedInstanceWeaponId = x.InstanceId;
                                    actor.SpecController.ChangeInstanceWeapon(x);
                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                    stateMachine.Change(StateHomeRoot);
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceWeapon", x);
                                    instanceWeaponSequences.PlayAsync(container, scope).Forget();
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
                instanceWeaponView.DestroySafe();
            }

            async UniTask StateChangeInstanceArmorRoot(CancellationToken scope)
            {
                SetHeaderText("防具変更");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "頭", _ =>
                            {
                                selectedArmorType = Define.ArmorType.Head;
                                stateMachine.Change(StateChangeInstanceArmor);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "腕", _ =>
                            {
                                selectedArmorType = Define.ArmorType.Arms;
                                stateMachine.Change(StateChangeInstanceArmor);
                            });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, "胴", _ =>
                            {
                                selectedArmorType = Define.ArmorType.Body;
                                stateMachine.Change(StateChangeInstanceArmor);
                            });
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

            async UniTask StateChangeInstanceArmor(CancellationToken scope)
            {
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceArmorView = UnityEngine.Object.Instantiate(instanceArmorViewDocumentPrefab);
                var instanceArmorSequences = instanceArmorView.Q<SequencesMonoBehaviour>("Sequences");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    userData.InstanceArmors
                        .Where(x => x.ArmorSpec.ArmorType == selectedArmorType)
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            var header = userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId == x.InstanceId
                                ? $"[E] {x.ArmorSpec.Name}"
                                : x.ArmorSpec.Name;
                            UIViewList.ApplyAsSimpleElement(document, header, _ =>
                                {
                                    var instanceId = userData.GetEquippedInstanceArmor(selectedArmorType)?.InstanceId ==
                                                     x.InstanceId ? 0 : x.InstanceId;
                                    userData.SetEquippedInstanceArmor(selectedArmorType, instanceId);
                                    actor.SpecController.BuildStatuses();
                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                    stateMachine.Change(StateChangeInstanceArmorRoot);
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceArmor", x);
                                    instanceArmorSequences.PlayAsync(container, scope).Forget();
                                });
                        })),
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateChangeInstanceArmorRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceArmorView.DestroySafe();
            }

            async UniTask StateRemoveInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("武器削除");
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                CancellationDisposable dialogScope = null;
                Selectable tempSelection = null;
                HKUIDocument list = null;
                CreateList();
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Where(_ => dialogScope == null)
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateHomeRoot);
                    })
                    .RegisterTo(scope);
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        userData.InstanceWeapons
                            .Where(x => x.InstanceId != userData.EquippedInstanceWeaponId)
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                UIViewList.ApplyAsSimpleElement(document, x.WeaponSpec.Name, async _ =>
                                {
                                    tempSelection = document.Q<Selectable>("Button");
                                    dialogScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
                                    var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>().OpenAsync(
                                        "本当に削除しますか？",
                                        new List<string> { "はい", "いいえ" },
                                        0,
                                        1,
                                        dialogScope.Token
                                    );
                                    dialogScope.Dispose();
                                    dialogScope = null;
                                    if (result == 0)
                                    {
                                        userData.RemoveInstanceWeapon(x);
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
                                });
                            })),
                        0
                    );
                }
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceWeaponView.DestroySafe();
            }

            async UniTask StateRemoveInstanceArmor(CancellationToken scope)
            {
                SetHeaderText("防具削除");
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceArmorView = UnityEngine.Object.Instantiate(instanceArmorViewDocumentPrefab);
                var instanceArmorSequences = instanceArmorView.Q<SequencesMonoBehaviour>("Sequences");
                CancellationDisposable dialogScope = null;
                Selectable tempSelection = null;
                HKUIDocument list = null;
                CreateList();
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Where(_ => dialogScope == null)
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateHomeRoot);
                    })
                    .RegisterTo(scope);
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        userData.InstanceArmors
                            .Where(x => x.ArmorSpec.ArmorType == selectedArmorType)
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                UIViewList.ApplyAsSimpleElement(document, x.ArmorSpec.Name, async _ =>
                                {
                                    tempSelection = document.Q<Selectable>("Button");
                                    dialogScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
                                    var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>().OpenAsync(
                                        "本当に削除しますか？",
                                        new List<string> { "はい", "いいえ" },
                                        0,
                                        1,
                                        dialogScope.Token
                                    );
                                    dialogScope.Dispose();
                                    dialogScope = null;
                                    if (result == 0)
                                    {
                                        var armorSpec = x.ArmorSpec;
                                        if (userData.GetEquippedInstanceArmor(armorSpec.ArmorType).InstanceId ==
                                            x.InstanceId)
                                        {
                                            userData.SetEquippedInstanceArmor(armorSpec.ArmorType, 0);
                                            actor.SpecController.BuildStatuses();
                                        }
                                        userData.RemoveInstanceArmor(x);
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
                                });
                            })),
                        0
                    );
                }
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceArmorView.DestroySafe();
            }

            async UniTask StateAddInstanceSkillCoreSelectInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("スキルコア装着");
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<UserData>().InstanceWeapons
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, x.WeaponSpec.Name, _ =>
                            {
                                if (x.SkillSlot <= 0)
                                {
                                    TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("スキルスロットがありません").Forget();
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
                            });
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
                await UniTask.WaitUntilCanceled(scope);
                instanceWeaponView.DestroySafe();
                list.DestroySafe();
            }

            async UniTask StateRemoveInstanceSkillCore(CancellationToken scope)
            {
                SetHeaderText("スキルコア削除");
                var userData = TinyServiceLocator.Resolve<UserData>();
                var instanceSkillCoreView = UnityEngine.Object.Instantiate(instanceSkillCoreViewDocumentPrefab);
                var instanceSkillCoreSequences = instanceSkillCoreView.Q<SequencesMonoBehaviour>("Sequences");
                CancellationDisposable dialogScope = null;
                HKUIDocument list = null;
                CreateList();
                void CreateList()
                {
                    if (list != null)
                    {
                        list.DestroySafe();
                    }
                    list = UIViewList.CreateWithPages(
                        listDocumentPrefab,
                        userData.InstanceSkillCores
                            .Select(x => new Action<HKUIDocument>(document =>
                            {
                                var header = x.SkillCoreSpec.Name;
                                UIViewList.ApplyAsSimpleElement(document, header, async _ =>
                                {
                                    var tempSelection = document.Q<Button>("Button");
                                    dialogScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
                                    var result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>().OpenAsync(
                                        "本当に削除しますか？",
                                        new List<string> { "はい", "いいえ" },
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
                                            dialogScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
                                            result = await TinyServiceLocator.Resolve<UIViewSimpleDialog>().OpenAsync(
                                                "武器に装着されてますが、本当に削除しますか？削除する場合、自動的に外れます",
                                                new List<string> { "はい", "いいえ" },
                                                0,
                                                1,
                                                dialogScope.Token
                                            );
                                            dialogScope.Dispose();
                                            dialogScope = null;
                                            if (result == 0)
                                            {
                                                userData.RemoveInstanceSkillCore(x);
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
                                            CreateList();
                                        }
                                    }
                                    else
                                    {
                                        tempSelection.Select();
                                    }
                                },
                                _ =>
                                {
                                    var container = new Container();
                                    container.Register("InstanceSkillCore", x);
                                    instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                                });
                            })),
                        0
                    );
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
                var instanceSkillCoreView = UnityEngine.Object.Instantiate(instanceSkillCoreViewDocumentPrefab);
                var instanceSkillCoreSequences = instanceSkillCoreView.Q<SequencesMonoBehaviour>("Sequences");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<UserData>().InstanceSkillCores
                        .OrderBy(x => selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId) ? 0 : 1)
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            var header = selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId)
                                ? $"[E] {x.SkillCoreSpec.Name}"
                                : x.SkillCoreSpec.Name;
                            UIViewList.ApplyAsSimpleElement(document, header, _ =>
                            {
                                var userData = TinyServiceLocator.Resolve<UserData>();
                                if (selectedInstanceWeapon.InstanceSkillCoreIds.Contains(x.InstanceId))
                                {
                                    selectedInstanceWeapon.RemoveInstanceSkillCoreId(x.InstanceId);
                                }
                                else
                                {
                                    if (selectedInstanceWeapon.GetUsingSlotCount(userData.InstanceSkillCores) + x.Slot > selectedInstanceWeapon.SkillSlot)
                                    {
                                        TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("スキルスロットが足りません").Forget();
                                        return;
                                    }
                                    selectedInstanceWeapon.AddInstanceSkillCoreId(x.InstanceId);
                                    TinyServiceLocator.Resolve<AudioManager>().PlaySfx("UI.Equipment.1");
                                }
                                if (userData.EquippedInstanceWeaponId == selectedInstanceWeapon.InstanceId)
                                {
                                    actor.SpecController.ChangeInstanceWeapon(selectedInstanceWeapon);
                                }
                                stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon);
                            },
                            _ =>
                            {
                                var container = new Container();
                                container.Register("InstanceSkillCore", x);
                                instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
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

                await UniTask.WaitUntilCanceled(scope);

                instanceSkillCoreView.DestroySafe();
                list.DestroySafe();
            }

            void SetHeaderText(string text)
            {
                header.Q<TMP_Text>("Header").text = text;
            }
        }
    }
}
