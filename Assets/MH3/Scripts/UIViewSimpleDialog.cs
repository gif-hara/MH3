using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace MH3
{
    public static class UIViewSimpleDialog
    {
        public static async UniTask<int> OpenAsync(
            HKUIDocument documentPrefab,
            string title,
            string message,
            IEnumerable<string> options,
            CancellationToken scope
            )
        {
            var document = Object.Instantiate(documentPrefab);
            document.Q<TMP_Text>("Title").text = title;
            document.Q<TMP_Text>("Message").text = message;
            var listParent = document.Q<RectTransform>("Parent.List");
            var elementPrefab = document.Q<HKUIDocument>("Prefab.Element");
            
            var result = await UniTask.WhenAny(options.Select(x =>
            {
                var element = Object.Instantiate(elementPrefab, listParent);
                element.Q<TMP_Text>("Text").text = x;
                return element.Q<Button>("Button").OnClickAsync(scope);
            }));

            document.DestroySafe();

            return result;
        }
    }
}
