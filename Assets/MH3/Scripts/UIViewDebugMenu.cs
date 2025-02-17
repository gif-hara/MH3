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
#if DEBUG
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
                                "クエスト".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateQuest);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "武器作成".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateCreateInstanceWeapon);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "防具作成".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateCreateInstanceArmor);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "スキルコア作成".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateCreateInstanceSkillCore);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "Add AvailableContents".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateAddAvailableContents);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "チェック".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateCheck);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "スキル作成".Localized(),
                                _ =>
                                {
                                    stateMachine.Change(StateCreateSkillCoreSelectSkill);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "閉じる".Localized(),
                                _ =>
                                {
                                    debugMenuScope.Dispose();
                                });
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => debugMenuScope.Dispose())
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
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
                                "無敵トグル.プレイヤー".Localized(),
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
                                "無敵トグル.エネミー".Localized(),
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
                                "ダメージゼロ.プレイヤー".Localized(),
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
                                "ダメージゼロ.エネミー".Localized(),
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
                                "瀕死.プレイヤー".Localized(),
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
                                "瀕死.エネミー".Localized(),
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
                                "強制怯み.プレイヤー".Localized(),
                                _ =>
                                {
                                    debugData.ForceFlinchSmallPlayer = !debugData.ForceFlinchSmallPlayer;
                                    Debug.Log($"ForceFlinchSmallPlayer: {debugData.ForceFlinchSmallPlayer}");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "強制怯み.エネミー".Localized(),
                                _ =>
                                {
                                    debugData.ForceFlinchSmallEnemy = !debugData.ForceFlinchSmallEnemy;
                                    Debug.Log($"ForceFlinchSmallEnemy: {debugData.ForceFlinchSmallEnemy}");
                                });
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
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
                                $"{questSpec.Id}: {questSpec.GetEnemyActorSpec().LocalizedName}",
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
                list.Dispose();
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
                                $"{x.Id}: {x.LocalizedName}",
                                _ =>
                                {
                                    var userData = TinyServiceLocator.Resolve<UserData>();
                                    userData.AddInstanceWeaponData(InstanceWeaponFactory.Create(userData, x.Id));
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
                list.Dispose();
            }

            async UniTask StateCreateInstanceArmor(CancellationToken scope)
            {
                var debugData = TinyServiceLocator.Resolve<GameDebugData>();
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().ArmorSpecs.List
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                $"{x.Id}: {x.LocalizedName}",
                                _ =>
                                {
                                    var userData = TinyServiceLocator.Resolve<UserData>();
                                    userData.AddInstanceArmor(InstanceArmorFactory.Create(userData, x.Id));
                                    Debug.Log($"Create InstanceArmorData: {x.Id}");
                                });
                        }))
                        .ToList(),
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
            }

            async UniTask StateCreateInstanceSkillCore(CancellationToken scope)
            {
                var debugData = TinyServiceLocator.Resolve<GameDebugData>();
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    TinyServiceLocator.Resolve<MasterData>().SkillCoreSpecs.List
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                $"{x.Id}: {x.LocalizedName}",
                                _ =>
                                {
                                    var userData = TinyServiceLocator.Resolve<UserData>();
                                    userData.AddInstanceSkillCore(InstanceSkillCoreFactory.Create(userData, x.Id));
                                    Debug.Log($"Create InstanceSkillCoreData: {x.Id}");
                                });
                        }))
                        .ToList(),
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
            }

            async UniTask StateAddAvailableContents(CancellationToken scope)
            {
                var elements = new List<string>();
                var masterData = TinyServiceLocator.Resolve<MasterData>();
                elements.AddRange(masterData.QuestSpecs.List.Select(x => AvailableContents.Key.GetQuestClear(x.Id)));
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    elements
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                x,
                                _ =>
                                {
                                    var userData = TinyServiceLocator.Resolve<UserData>();
                                    userData.AvailableContents.Add(x);
                                    Debug.Log($"Add AvailableContents: {x}");
                                });
                        }))
                        .ToList(),
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
            }

            async UniTask StateCheck(CancellationToken scope)
            {
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    new List<Action<HKUIDocument>>
                    {
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "Print UserData Json",
                                _ =>
                                {
                                    var json = JsonUtility.ToJson(TinyServiceLocator.Resolve<UserData>(), true);
                                    Debug.Log(json);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "Print SaveData Json",
                                _ =>
                                {
                                    var json = JsonUtility.ToJson(TinyServiceLocator.Resolve<SaveData>(), true);
                                    Debug.Log(json);
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "Save",
                                _ =>
                                {
                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                    Debug.Log("Save");
                                });
                        },
                        document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                "Delete",
                                _ =>
                                {
                                    SaveSystem.Delete(SaveData.Path);
                                    Debug.Log("Delete");
                                });
                        },
                    },
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
            }

            async UniTask StateCreateSkillCoreSelectSkill(CancellationToken scope)
            {
                var list = UIViewList.CreateWithPages(
                    listDocumentPrefab,
                    Enum.GetValues(typeof(Define.SkillType)).Cast<Define.SkillType>()
                        .Select(x => new Action<HKUIDocument>(document =>
                        {
                            UIViewList.ApplyAsSimpleElement(
                                document,
                                $"{x.GetName()}",
                                _ =>
                                {
                                    var userData = TinyServiceLocator.Resolve<UserData>();
                                    var instanceSkill = new InstanceSkill(x, 99, Define.RareType.Common);
                                    var instanceSkillCore = new InstanceSkillCore(
                                        userData.GetAndIncrementCreatedInstanceSkillCoreCount(),
                                        19901,
                                        1,
                                        new List<InstanceSkill> { instanceSkill }
                                        );
                                    userData.AddInstanceSkillCore(instanceSkillCore);
                                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                                    Debug.Log($"Create InstanceSkillCoreData: {x}");
                                });
                        })),
                    0
                );
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ => stateMachine.Change(StateRoot))
                    .RegisterTo(scope);
                await UniTask.WaitUntilCanceled(scope);
                list.Dispose();
            }
        }
    }
#endif
}
