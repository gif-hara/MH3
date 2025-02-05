using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using HK;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnitySequencerSystem;

namespace MH3
{
    public class UIViewAcquireReward
    {
        public static async UniTask<int> OpenAsync(HKUIDocument documentPrefab, List<IReward> rewards, float elapsedTime, string enemyName, CancellationToken scope)
        {
            if (!rewards.Any())
            {
                return -1;
            }

            var document = UnityEngine.Object.Instantiate(documentPrefab);
            var elementParent = document.Q<RectTransform>("Parent.Element");
            var selectable = new List<Selectable>();
            var source = new UniTaskCompletionSource<int>();
            document.Q<TMP_Text>("EnemyName").text = enemyName;
            var timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
            document.Q<TMP_Text>("ClearTime").text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D3}";
            var selectScope = new CancellationTokenSource();
            foreach (var reward in rewards)
            {
                switch (reward)
                {
                    case InstanceWeapon instanceWeapon:
                        {
                            var element = UnityEngine.Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceWeapon"), elementParent);
                            var container = new Container();
                            container.Register("InstanceWeapon", instanceWeapon);
                            element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                            var button = element.Q<HKUIDocument>("Element.Button").Q<Button>("Button");
                            button.OnClickAsObservable()
                                .Subscribe(_ =>
                                {
                                    source.TrySetResult(selectable.IndexOf(button));
                                    PlayAcquireEffect(element);
                                })
                                .RegisterTo(selectScope.Token);
                            selectable.Add(button);
                            break;
                        }
                    case InstanceSkillCore instanceSkillCore:
                        {
                            var element = UnityEngine.Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceSkillCore"), elementParent);
                            var container = new Container();
                            container.Register("InstanceSkillCore", instanceSkillCore);
                            element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                            var button = element.Q<HKUIDocument>("Element.Button").Q<Button>("Button");
                            button.OnClickAsObservable()
                                .Subscribe(_ =>
                                {
                                    source.TrySetResult(selectable.IndexOf(button));
                                    PlayAcquireEffect(element);
                                })
                                .RegisterTo(selectScope.Token);
                            selectable.Add(button);
                            break;
                        }
                    case InstanceArmor instanceArmor:
                        {
                            var element = UnityEngine.Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceArmor"), elementParent);
                            var container = new Container();
                            container.Register("InstanceArmor", instanceArmor);
                            element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                            var button = element.Q<HKUIDocument>("Element.Button").Q<Button>("Button");
                            button.OnClickAsObservable()
                                .Subscribe(_ =>
                                {
                                    source.TrySetResult(selectable.IndexOf(button));
                                    PlayAcquireEffect(element);
                                })
                                .RegisterTo(selectScope.Token);
                            selectable.Add(button);
                            break;
                        }
                    default:
                        throw new System.NotImplementedException($"未対応のIRewardです {reward.GetType()}");
                }
            }

            selectable.SetNavigationHorizontal();
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var defaultSelectable = selectable.First();
            EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
            var result = await source.Task;
            selectScope.Cancel();
            selectScope.Dispose();
            foreach (var i in selectable)
            {
                i.navigation = new Navigation();
            }

            // 獲得エフェクトアニメーションが終わるまで待つ
            await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: scope);

            inputController.PopActionType();
            document.DestroySafe();
            return result;

            static void PlayAcquireEffect(HKUIDocument element)
            {
                var effect = element.Q<HKUIDocument>("AcquireEffect");
                effect.gameObject.SetActive(true);
                effect
                    .Q<SimpleAnimation>("SimpleAnimation")
                    .Play("Default");
            }
        }
    }
}
