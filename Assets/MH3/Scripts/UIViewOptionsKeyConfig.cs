using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;

namespace MH3
{
    public class UIViewOptionsKeyConfig : IUIViewOptions
    {
        private HKUIDocument document;

        public UIViewOptionsKeyConfig(HKUIDocument documentPrefab)
        {
            document = Object.Instantiate(documentPrefab);
        }

        public async UniTask ActivateAsync(CancellationToken scope)
        {
            var inputController = TinyServiceLocator.Resolve<InputController>();
            await inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .FirstAsync();
        }

        public void Dispose()
        {
            document.DestroySafe();
        }
    }
}
