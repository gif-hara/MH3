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
using static UnityEngine.InputSystem.InputAction;

namespace MH3
{
    public class UIViewPauseMenu
    {
        public static async UniTask OpenAsync(
            HKUIDocument headerDocumentPrefab,
            HKUIDocument listDocumentPrefab,
            HKUIDocument instanceWeaponViewDocumentPrefab,
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
            string selectInstanceWeaponHeader = "";
            Action<InstanceWeaponData> selectInstanceWeaponOnClickAction = null;
            Action<CallbackContext> selectInstanceWeaponOnCancelAction = null;
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
                                selectInstanceWeaponHeader = "武器変更";
                                selectInstanceWeaponOnClickAction = x =>
                                {
                                    actor.SpecController.ChangeInstanceWeapon(x);
                                };
                                selectInstanceWeaponOnCancelAction = _ => stateMachine.Change(StateHomeRoot);
                                stateMachine.Change(StateSelectInstanceWeapon);
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

            async UniTask StateSelectInstanceWeapon(CancellationToken scope)
            {
                SetHeaderText(selectInstanceWeaponHeader);
                var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
                var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<UserData>().InstanceWeaponDataList
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(document, x.WeaponSpec.Name, _ =>
                            {
                                selectInstanceWeaponOnClickAction(x);
                            },
                            _ =>
                            {
                                var container = new Container();
                                container.Register("InstanceWeaponData", x);
                                instanceWeaponSequences.PlayAsync(container, scope).Forget();
                            });
                        })),
                    0
                );
                inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .Subscribe(selectInstanceWeaponOnCancelAction)
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.DestroySafe();
                instanceWeaponView.DestroySafe();
            }

            void SetHeaderText(string text)
            {
                header.Q<TMP_Text>("Header").text = text;
            }
        }
    }
}
