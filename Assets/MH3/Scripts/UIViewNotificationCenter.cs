using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;

namespace MH3
{
    public class UIViewNotificationCenter
    {
        private readonly HKUIDocument document;

        public UIViewNotificationCenter(HKUIDocument documentPrefab, CancellationToken scope)
        {
            document = Object.Instantiate(documentPrefab);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
            document.Q<CanvasGroup>("Area.Root").alpha = 0.0f;
        }

        public UniTask BeginOneShotAsync(string message)
        {
            document.Q<TMP_Text>("Message").text = message;
            return document.Q<SimpleAnimation>("Animation").PlayAsync("Default", document.destroyCancellationToken);
        }
    }
}
