using System;
using System.Collections.Generic;
using System.Linq;
using HK;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MH3
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UIViewList : IDisposable
    {
        private readonly HKUIDocument document;

        private readonly List<Selectable> buttons = new();

        private UIViewList(HKUIDocument document)
        {
            this.document = document;
        }

        public static UIViewList CreateWithPages
        (
            HKUIDocument listDocumentPrefab,
            IEnumerable<Action<HKUIDocument>> elementActivateActions,
            int initialElementIndex,
            bool canSetSelectedGameObject = true
        )
        {
            var document = Object.Instantiate(listDocumentPrefab);
            var result = new UIViewList(document);
            var listParent = document.Q<RectTransform>("Parent.List");
            var layoutGroup = document.Q<VerticalLayoutGroup>("Parent.List");
            var listElementPrefab = document.Q<HKUIDocument>("Prefab.Element");
            var parentSize = listParent.rect.height - layoutGroup.padding.top - layoutGroup.padding.bottom;
            var elementSize = ((RectTransform)listElementPrefab.transform).rect.height + layoutGroup.spacing;
            var elementCount = Mathf.FloorToInt(parentSize / elementSize);
            var pageIndex = initialElementIndex / elementCount;
            var pageMax = elementActivateActions.Count() / elementCount;
            if (elementActivateActions.Count() % elementCount == 0)
            {
                pageMax--;
            }
            var elementIndex = 0;
            var elements = new List<HKUIDocument>();
            var emptyArea = document.TryQ("Area.Empty");
            if (emptyArea != null)
            {
                emptyArea.SetActive(!elementActivateActions.Any());
            }
            Selectable defaultSelectable = null;
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.Actions.UI.Navigate
                .OnPerformedAsObservable()
                .Subscribe(x =>
                {
                    if (EventSystem.current.currentSelectedGameObject != null || defaultSelectable == null)
                    {
                        return;
                    }
                    EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
                })
                .RegisterTo(document.destroyCancellationToken);
            CreateList(initialElementIndex % elementCount, canSetSelectedGameObject);

            void CreateList(int selectIndex, bool canSetSelectedGameObject)
            {
                foreach (var element in elements)
                {
                    Object.Destroy(element.gameObject);
                }
                elements.Clear();
                elementIndex = 0;
                result.buttons.Clear();
                foreach (var action in elementActivateActions.Skip(pageIndex * elementCount).Take(elementCount))
                {
                    var element = Object.Instantiate(listElementPrefab, listParent);
                    elements.Add(element);
                    var button = element.Q<Button>("Button");
                    result.buttons.Add(button);
                    action(element);
                    button.OnSelectAsObservable()
                        .Subscribe(_ =>
                        {
                            TinyServiceLocator.Resolve<InputController>().Actions.UI.Navigate.OnPerformedAsObservable()
                                .TakeUntil(button.OnDeselectAsObservable())
                                .Subscribe(x =>
                                {
                                    if (pageMax == 0)
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
                                        pageIndex = (pageIndex + 1) % (pageMax + 1);
                                        CreateList(0, true);
                                    }
                                    else if (direction.x < 0)
                                    {
                                        pageIndex = pageIndex == 0 ? pageMax : pageIndex - 1;
                                        CreateList(0, true);
                                    }
                                })
                                .RegisterTo(element.destroyCancellationToken);
                        })
                        .RegisterTo(element.destroyCancellationToken);
                    if (elementIndex == selectIndex)
                    {
                        if (canSetSelectedGameObject)
                        {
                            EventSystem.current.SetSelectedGameObject(button.gameObject);
                        }
                        defaultSelectable = button;
                    }
                    elementIndex++;
                }
                result.buttons.SetNavigationVertical();
                UpdatePage(pageIndex);
            }
            void UpdatePage(int index)
            {
                var pageArea = document.TryQ("Area.Page");
                if (pageArea == null)
                {
                    return;
                }
                if (pageMax <= 0)
                {
                    pageArea.SetActive(false);
                }
                else
                {
                    pageArea.SetActive(true);
                    document.Q<TMP_Text>("Text.Page").text = $"{index + 1}/{pageMax + 1}";
                }
            }
            return result;
        }

        public void Dispose()
        {
            document.DestroySafe();
        }

        public void SetSelectable(int index)
        {
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }

        public void SetEmptyAreaMessage(string message)
        {
            document.Q<TMP_Text>("Text.Empty").text = message;
        }

        public static void ApplyAsSimpleElement
        (
            HKUIDocument element,
            string header,
            Action<Unit> onClick,
            Action<BaseEventData> onSelect = null,
            Action<BaseEventData> onDeselect = null
        )
        {
            element.Q<TMP_Text>("Header").text = header;
            var button = element.Q<Button>("Button");
            button.OnClickAsObservable()
                .Subscribe(onClick)
                .RegisterTo(element.destroyCancellationToken);
            if (onSelect != null)
            {
                button.OnSelectAsObservable()
                    .Subscribe(onSelect)
                    .RegisterTo(element.destroyCancellationToken);
            }
            if (onDeselect != null)
            {
                button.OnDeselectAsObservable()
                    .Subscribe(onDeselect)
                    .RegisterTo(element.destroyCancellationToken);
            }
        }
    }
}
