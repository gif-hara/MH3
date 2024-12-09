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
using UnitySequencerSystem;

namespace MH3
{
    public class UIViewPauseMenu
    {
        public static async UniTask OpenAsync(
            HKUIDocument headerDocumentPrefab,
            HKUIDocument listDocumentPrefab,
            HKUIDocument instanceWeaponViewDocumentPrefab,
            HKUIDocument instanceSkillCoreViewDocumentPrefab,
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
                            UIViewList.ApplyAsSimpleElement(document, "スキルコア装着", _ =>
                            {
                                stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon);
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
                SetHeaderText("クエスト選択");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().QuestSpecs.List
                        .Where(x => x.AvailableQuestList)
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, x.Id, _ =>
                            {
                                gameSceneController.SetupQuestAsync(x.Id).Forget();
                                pauseMenuScope.Dispose();
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
            }

            UniTask StateChangeInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("武器変更");
                var selectInstanceWeaponViewScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                return UIViewSelectInstanceWeapon.OpenAsync(
                    listDocumentPrefab,
                    instanceWeaponViewDocumentPrefab,
                    x =>
                    {
                        var userData = TinyServiceLocator.Resolve<UserData>();
                        if (userData.EquippedInstanceWeaponId == x.InstanceId)
                        {
                            TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("既に装備しています").Forget();
                            return;
                        }
                        userData.EquippedInstanceWeaponId = x.InstanceId;
                        actor.SpecController.ChangeInstanceWeapon(x, TinyServiceLocator.Resolve<UserData>().InstanceSkillCoreList);
                        selectInstanceWeaponViewScope.Cancel();
                        selectInstanceWeaponViewScope.Dispose();
                        stateMachine.Change(StateHomeRoot);
                    },
                    _ =>
                    {
                        selectInstanceWeaponViewScope.Cancel();
                        selectInstanceWeaponViewScope.Dispose();
                        stateMachine.Change(StateHomeRoot);
                    },
                    selectInstanceWeaponViewScope.Token
                );
            }

            UniTask StateAddInstanceSkillCoreSelectInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText("スキルコア装着");
                var selectInstanceWeaponViewScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
                return UIViewSelectInstanceWeapon.OpenAsync(
                    listDocumentPrefab,
                    instanceWeaponViewDocumentPrefab,
                    x =>
                    {
                        if (x.SkillSlot <= 0)
                        {
                            TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("スキルスロットがありません").Forget();
                            return;
                        }
                        selectedInstanceWeapon = x;
                        stateMachine.Change(StateAddInstanceSkillCoreSelectSkillCore);
                        selectInstanceWeaponViewScope.Cancel();
                        selectInstanceWeaponViewScope.Dispose();
                    },
                    _ =>
                    {
                        selectInstanceWeaponViewScope.Cancel();
                        selectInstanceWeaponViewScope.Dispose();
                        stateMachine.Change(StateHomeRoot);
                    },
                    selectInstanceWeaponViewScope.Token
                );
            }

            async UniTask StateAddInstanceSkillCoreSelectSkillCore(CancellationToken scope)
            {
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(_ =>
                    {
                        stateMachine.Change(StateAddInstanceSkillCoreSelectInstanceWeapon);
                    })
                    .RegisterTo(scope);

                var instanceSkillCoreView = UnityEngine.Object.Instantiate(instanceSkillCoreViewDocumentPrefab);
                var instanceSkillCoreSequences = instanceSkillCoreView.Q<SequencesMonoBehaviour>("Sequences");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<UserData>().InstanceSkillCoreList
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
                                    if (selectedInstanceWeapon.GetUsingSlotCount(userData.InstanceSkillCoreList) + x.Slot > selectedInstanceWeapon.SkillSlot)
                                    {
                                        TinyServiceLocator.Resolve<UIViewNotificationCenter>().BeginOneShotAsync("スキルスロットが足りません").Forget();
                                        return;
                                    }
                                    selectedInstanceWeapon.AddInstanceSkillCoreId(x.InstanceId);
                                }
                                if (userData.EquippedInstanceWeaponId == selectedInstanceWeapon.InstanceId)
                                {
                                    actor.SpecController.ChangeInstanceWeapon(selectedInstanceWeapon, userData.InstanceSkillCoreList);
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
