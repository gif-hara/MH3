using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using TMPro;
using UnityEngine;

namespace MH3
{
    public class UIViewDebugMenu
    {
        public static async UniTask OpenAsync(
            HKUIDocument headerDocumentPrefab,
            HKUIDocument listDocumentPrefab,
            GameSceneController gameSceneController,
            Actor player,
            Actor enemy,
            CancellationToken scope
            )
        {
            var debugMenuScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
            var tempTimeScale = HK.Time.Root.timeScale;
            HK.Time.Root.timeScale = 0.0f;
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var stateMachine = new TinyStateMachine();
            stateMachine.Change(StateRoot);
            var header = UnityEngine.Object.Instantiate(headerDocumentPrefab);
            var headerTexts = new List<string>();
            PushHeaderText("Debug Menu");

            // 待機
            {
                await UniTask.WaitUntilCanceled(debugMenuScope.Token);
            }

            // 終了処理
            {
                inputController.PopActionType();
                HK.Time.Root.timeScale = tempTimeScale;
                stateMachine.Dispose();
                if (header != null)
                {
                    UnityEngine.Object.Destroy(header.gameObject);
                }
            }

            async UniTask StateRoot(CancellationToken scope)
            {
                var debugData = TinyServiceLocator.Resolve<GameDebugData>();
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "無敵トグル.プレイヤー",
                                _ =>
                                {
                                    debugData.InvinciblePlayer = !debugData.InvinciblePlayer;
                                    Debug.Log($"InvinciblePlayer: {debugData.InvinciblePlayer}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "無敵トグル.エネミー",
                                _ =>
                                {
                                    debugData.InvincibleEnemy = !debugData.InvincibleEnemy;
                                    Debug.Log($"InvincibleEnemy: {debugData.InvincibleEnemy}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "ダメージゼロ.プレイヤー",
                                _ =>
                                {
                                    debugData.DamageZeroPlayer = !debugData.DamageZeroPlayer;
                                    Debug.Log($"DamageZeroPlayer: {debugData.DamageZeroPlayer}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "ダメージゼロ.エネミー",
                                _ =>
                                {
                                    debugData.DamageZeroEnemy = !debugData.DamageZeroEnemy;
                                    Debug.Log($"DamageZeroEnemy: {debugData.DamageZeroEnemy}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "瀕死.プレイヤー",
                                _ =>
                                {
                                    player.SpecController.SetHitPointDebug(1);
                                    Debug.Log($"Player HitPoint: {player.SpecController.HitPoint.CurrentValue}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "瀕死.エネミー",
                                _ =>
                                {
                                    enemy.SpecController.SetHitPointDebug(1);
                                    Debug.Log($"Enemy HitPoint: {enemy.SpecController.HitPoint.CurrentValue}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "クエスト.ホーム",
                                _ =>
                                {
                                    gameSceneController.SetupHomeQuestAsync();
                                    debugMenuScope.Dispose();
                                    Debug.Log("Setup Home Quest");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "クエスト.デフォルト",
                                _ =>
                                {
                                    gameSceneController.SetupDefaultQuestAsync();
                                    debugMenuScope.Dispose();
                                    Debug.Log("Setup Default Quest");
                                });
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => debugMenuScope.Dispose())
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                if (list != null)
                {
                    UnityEngine.Object.Destroy(list.gameObject);
                }
            }

            void PushHeaderText(string text)
            {
                headerTexts.Add(text);
                header.Q<TMP_Text>("Header").text = string.Join("/", headerTexts);
            }

            void PopHeaderText()
            {
                headerTexts.RemoveAt(headerTexts.Count - 1);
                header.Q<TMP_Text>("Header").text = string.Join("/", headerTexts);
            }
        }
    }
}
