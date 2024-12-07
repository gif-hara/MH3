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

namespace MH3
{
    public class UIViewPauseMenu
    {
        public static async UniTask OpenAsync(
            HKUIDocument headerDocumentPrefab,
            HKUIDocument listDocumentPrefab,
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
                                gameSceneController.SetupHomeQuest();
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
                                gameSceneController.SetupQuest(x.Id);
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

            void SetHeaderText(string text)
            {
                header.Q<TMP_Text>("Header").text = text;
            }
        }
    }
}
