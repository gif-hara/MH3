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
        public static async UniTask OpenAsync(
            HKUIDocument documentPrefab,
            IEnumerable<InstanceWeapon> instanceWeapons,
            IEnumerable<InstanceSkillCore> instanceSkillCores,
            CancellationToken scope
            )
        {
            if (instanceWeapons.Any() == false && instanceSkillCores.Any() == false)
            {
                return;
            }
            var document = Object.Instantiate(documentPrefab);
            var header = document.Q<TMP_Text>("Header");
            var areaInstanceWeapon = document.Q<HKUIDocument>("Area.InstanceWeapon");
            var instanceWeaponSequences = areaInstanceWeapon.Q<SequencesMonoBehaviour>("Sequences");
            var areaInstanceSkillCore = document.Q<HKUIDocument>("Area.InstanceSkillCore");
            var instanceSkillCoreSequences = areaInstanceSkillCore.Q<SequencesMonoBehaviour>("Sequences");
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);

            areaInstanceWeapon.gameObject.SetActive(true);
            areaInstanceSkillCore.gameObject.SetActive(false);
            foreach (var instanceWeapon in instanceWeapons)
            {
                var container = new Container();
                header.text = "武器獲得！";
                container.Register("InstanceWeapon", instanceWeapon);
                instanceWeaponSequences.PlayAsync(container, scope).Forget();
                await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(scope).AsUniTask();
            }

            areaInstanceWeapon.gameObject.SetActive(false);
            areaInstanceSkillCore.gameObject.SetActive(true);
            foreach (var instanceSkillCore in instanceSkillCores)
            {
                var container = new Container();
                header.text = "スキルコア獲得！";
                container.Register("InstanceSkillCore", instanceSkillCore);
                instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(scope).AsUniTask();
            }

            inputController.PopActionType();
            document.DestroySafe();
        }

        public static async UniTask<int> OpenAsync(HKUIDocument documentPrefab, List<IReward> rewards, CancellationToken scope)
        {
            if (!rewards.Any())
            {
                return -1;
            }

            var document = Object.Instantiate(documentPrefab);
            var elementParent = document.Q<RectTransform>("Parent.Element");
            var selectable = new List<Selectable>();
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
                        selectable.Add(element.Q<Button>("Button"));
                        break;
                    }
                    case InstanceSkillCore instanceSkillCore:
                    {
                        var element = Object.Instantiate(document.Q<HKUIDocument>("Prefab.InstanceSkillCore"), elementParent);
                        var container = new Container();
                        container.Register("InstanceSkillCore", instanceSkillCore);
                        element.Q<SequencesMonoBehaviour>("Sequences").PlayAsync(container, scope).Forget();
                        selectable.Add(element.Q<Button>("Button"));
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
            }
            
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var defaultSelectable = selectable.First();
            EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
            var index = 0;
            inputController.Actions.UI.Navigate.OnPerformedAsObservable()
                .Subscribe(x =>
                {
                    if (selectable.Count == 1)
                    {
                        return;
                    }
                    var direction = x.ReadValue<Vector2>();
                    if (direction.x == 0)
                    {
                        return;
                    }
                    if (direction.x > 0)
                    {
                        index = (index + 1) % selectable.Count;
                        EventSystem.current.SetSelectedGameObject(selectable[index].gameObject);
                    }
                    else if (direction.x < 0)
                    {
                        index = index == 0 ? selectable.Count - 1 : index - 1;
                        EventSystem.current.SetSelectedGameObject(selectable[index].gameObject);
                    }
                })
                .RegisterTo(scope);
            await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(scope).AsUniTask();
            inputController.PopActionType();
            document.DestroySafe();
            return index;
        }
    }
}
