using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace MH3
{
    public class UIViewFade
    {
        private readonly HKUIDocument document;

        public UIViewFade(HKUIDocument documentPrefab, CancellationToken scope)
        {
            document = Object.Instantiate(documentPrefab);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
            document.Q<CanvasGroup>("Area.Root").alpha = 0.0f;
        }

        public UniTask BeginAnimation(string key)
        {
            return document.Q<SimpleAnimation>("Animation").PlayAsync(key, document.destroyCancellationToken);
        }
    }
}
