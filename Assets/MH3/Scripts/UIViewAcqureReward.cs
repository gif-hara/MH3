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

            var document = Object.Instantiate(documentPrefab);
            var elementParent = document.Q<RectTransform>("Parent.Element");
            var selectable = new List<Selectable>();
            var source = new UniTaskCompletionSource<int>();
            document.Q<TMP_Text>("EnemyName").text = enemyName;
            var timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
            document.Q<TMP_Text>("ClearTime").text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D3}";
            foreach (var reward in rewards)
            {
                switch (reward)
                {
                    case InstanceWeapon instanceWeapon:
                        {
                            var element = Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceWeapon"), elementParent);
                            var container = new Container();
                            container.Register("InstanceWeapon", instanceWeapon);
                            element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                            var button = element.Q<Button>("Button");
                            button.OnClickAsObservable()
                                .Subscribe(_ =>
                                {
                                    source.TrySetResult(selectable.IndexOf(button));
                                })
                                .RegisterTo(document.destroyCancellationToken);
                            selectable.Add(button);
                            break;
                        }
                    case InstanceSkillCore instanceSkillCore:
                        {
                            var element = Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceSkillCore"), elementParent);
                            var container = new Container();
                            container.Register("InstanceSkillCore", instanceSkillCore);
                            element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                            var button = element.Q<Button>("Button");
                            button.OnClickAsObservable()
                                .Subscribe(_ =>
                                {
                                    source.TrySetResult(selectable.IndexOf(button));
                                })
                                .RegisterTo(document.destroyCancellationToken);
                            selectable.Add(button);
                            break;
                        }
                    case InstanceArmor instanceArmor:
                        {
                            var element = Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceArmor"), elementParent);
                            var container = new Container();
                            container.Register("InstanceArmor", instanceArmor);
                            element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                            var button = element.Q<Button>("Button");
                            button.OnClickAsObservable()
                                .Subscribe(_ =>
                                {
                                    source.TrySetResult(selectable.IndexOf(button));
                                })
                                .RegisterTo(document.destroyCancellationToken);
                            selectable.Add(button);
                            break;
                        }
                    default:
                        throw new System.NotImplementedException($"未対応のIRewardです {reward.GetType()}");
                }
            }

            for (var i = 0; i < selectable.Count; i++)
            {
                var navigation = selectable[i].navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnLeft = selectable[((i - 1) % selectable.Count + selectable.Count) % selectable.Count];
                navigation.selectOnRight = selectable[(i + 1) % selectable.Count];
                selectable[i].navigation = navigation;
            }

            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var defaultSelectable = selectable.First();
            EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
            var result = await source.Task;
            inputController.PopActionType();
            document.DestroySafe();
            return result;
        }
    }
}
