using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using R3;
using UnityEngine;

namespace MH3
{
    public class UIViewTitle
    {
        public static async UniTask OpenAsync(HKUIDocument documentPrefab, CancellationToken cancellationToken)
        {
            var document = Object.Instantiate(documentPrefab);
            var inputController = TinyServiceLocator.Resolve<InputController>();
            var areaTitleCanvasGroup = document.Q<CanvasGroup>("Area.Title");
            inputController.PushActionType(InputController.InputActionType.UI);
            areaTitleCanvasGroup.alpha = 0.0f;
            await TinyServiceLocator.Resolve<UIViewTransition>()
                .Build()
                .SetMaterial("Transition.3")
                .BeginAsync(LMotion.Create(1.0f, 0.0f, 0.5f));
            await document.Q<SimpleAnimation>("Area.Title").PlayAsync("In", cancellationToken);
            await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(cancellationToken);
            await document.Q<SimpleAnimation>("Area.Title").PlayAsync("Out", cancellationToken);
            inputController.PopActionType();
            document.DestroySafe();
        }
    }
}
