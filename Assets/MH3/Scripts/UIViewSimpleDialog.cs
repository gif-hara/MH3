using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace MH3
{
    public class UIViewSimpleDialog
    {
        private readonly HKUIDocument documentPrefab;

        public UIViewSimpleDialog(HKUIDocument documentPrefab)
        {
            this.documentPrefab = documentPrefab;
        }

        public async UniTask<int> OpenAsync(
            string message,
            IEnumerable<string> options,
            int initialSelectionIndex,
            int cancelIndex,
            CancellationToken scope
            )
        {
            var document = Object.Instantiate(documentPrefab);
            var inputController = TinyServiceLocator.Resolve<InputController>();
            try
            {
                document.Q<TMP_Text>("Message").text = message;
                var listParent = document.Q<RectTransform>("Parent.List");
                var elementPrefab = document.Q<HKUIDocument>("Prefab.Element");
                var source = new UniTaskCompletionSource<int>();
                var buttons = options.Select((x, index) =>
                {
                    var element = Object.Instantiate(elementPrefab, listParent);
                    element.Q<TMP_Text>("Header").text = x;
                    var button = element.Q<Button>("Button");
                    button.OnClickAsObservable()
                        .Subscribe((source, index), static (_, t) =>
                        {
                            var (source, index) = t;
                            source.TrySetResult(index);
                        });
                    return button;
                })
                .ToList();
                inputController.PushActionType(InputController.InputActionType.UI);
                inputController.Actions.UI.Cancel.OnPerformedAsObservable()
                    .Subscribe(_ =>
                    {
                        source.TrySetResult(cancelIndex);
                    })
                    .RegisterTo(scope);
                for (var i = 0; i < buttons.Count; i++)
                {
                    var navigation = buttons[i].navigation;
                    navigation.mode = Navigation.Mode.Explicit;
                    if (i + 1 > buttons.Count - 1)
                    {
                        navigation.selectOnRight = buttons[0];
                    }
                    else
                    {
                        navigation.selectOnRight = buttons[i + 1];
                    }
                    if (i - 1 < 0)
                    {
                        navigation.selectOnLeft = buttons[buttons.Count - 1];
                    }
                    else
                    {
                        navigation.selectOnLeft = buttons[i - 1];
                    }
                    buttons[i].navigation = navigation;
                }
                buttons[initialSelectionIndex].Select();

                return await source.Task;
            }
            finally
            {
                document.DestroySafe();
                inputController.PopActionType();
            }
        }
    }
}
