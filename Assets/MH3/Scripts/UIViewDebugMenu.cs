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
            header.Q<TMP_Text>("Header").text = "Debug Menu";

            // 待機
            {
                await UniTask.WaitUntilCanceled(debugMenuScope.Token);
            }

            // 終了処理
            {
                inputController.PopActionType();
                HK.Time.Root.timeScale = tempTimeScale;
                stateMachine.Dispose();
                header.gameObject.DestroySafe();
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
                                "バトル",
                                _ =>
                                {
                                    stateMachine.Change(StateBattle);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "クエスト",
                                _ =>
                                {
                                    stateMachine.Change(StateQuest);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "武器作成",
                                _ =>
                                {
                                    stateMachine.Change(StateCreateInstanceWeapon);
                                });
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => debugMenuScope.Dispose())
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.gameObject.DestroySafe();
            }

            async UniTask StateBattle(CancellationToken scope)
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
                    },
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.gameObject.DestroySafe();
            }

            async UniTask StateQuest(CancellationToken scope)
            {
                var debugData = TinyServiceLocator.Resolve<GameDebugData>();
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().QuestSpecs.List
                        .Select(questSpec => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                questSpec.Id,
                                _ =>
                                {
                                    gameSceneController.SetupQuestAsync(questSpec.Id).Forget();
                                    debugMenuScope.Dispose();
                                });
                        }))
                        .ToList(),
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.gameObject.DestroySafe();
            }
            async UniTask StateCreateInstanceWeapon(CancellationToken scope)
            {
                var debugData = TinyServiceLocator.Resolve<GameDebugData>();
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.List
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                $"{x.Id}: {x.Name}",
                                _ =>
                                {
                                    TinyServiceLocator.Resolve<UserData>().AddInstanceWeaponData(InstanceWeaponFactory.Create(x.Id));
                                    Debug.Log($"Create InstanceWeaponData: {x.Id}");
                                });
                        }))
                        .ToList(),
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.gameObject.DestroySafe();
            }
        }
    }
}
