using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace MH3
{
    public class UIViewSimpleDialog
    {
        private readonly HKUIDocument documentPrefab;

        public UIViewSimpleDialog(HKUIDocument documentPrefab, CancellationToken scope)
        {
            this.documentPrefab = documentPrefab;
            scope.RegisterWithoutCaptureExecutionContext(() => documentPrefab.DestroySafe());
        }

        public async UniTask<int> OpenAsync(
            string message,
            IEnumerable<string> options,
            CancellationToken scope
            )
        {
            var document = Object.Instantiate(documentPrefab);
            document.Q<TMP_Text>("Message").text = message;
            var listParent = document.Q<RectTransform>("Parent.List");
            var elementPrefab = document.Q<HKUIDocument>("Prefab.Element");
            var buttons = options.Select(x =>
            {
                var element = Object.Instantiate(elementPrefab, listParent);
                element.Q<TMP_Text>("Text").text = x;
                return element.Q<Button>("Button");
            })
            .ToList();
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

            var result = await UniTask.WhenAny(
                buttons.Select((x, i) => x.OnClickAsync())
            );

            document.DestroySafe();

            return result;
        }
    }
}
