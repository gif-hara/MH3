using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using R3;
using TMPro;
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
            var areaTitleAnimation = document.Q<SimpleAnimation>("Area.Title");
            var areaPressButtonAnimation = document.Q<SimpleAnimation>("Area.PressButton");
            var areaPressButtonCanvasGroup = document.Q<CanvasGroup>("Area.PressButton");
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            var gameEvents = TinyServiceLocator.Resolve<GameEvents>();
            inputController.PushActionType(InputController.InputActionType.UI);
            areaTitleCanvasGroup.alpha = 0.0f;
            areaPressButtonCanvasGroup.alpha = 0.0f;
            gameCameraController.BeginTitle();
            TinyServiceLocator.Resolve<InputScheme>().InputSchemeTypeReactiveProperty
                .Subscribe((document, inputController), static (x, t) =>
                {
                    var (document, inputController) = t;
                    document.Q<TMP_Text>("Text.PressButton").text = string.Format("{0}:ゲームスタート", InputSprite.GetTag(inputController.Actions.UI.Submit)).Localized();
                })
                .RegisterTo(cancellationToken);
            gameEvents.OnBeginTitle.OnNext(Unit.Default);
            await TinyServiceLocator.Resolve<UIViewTransition>()
                .Build()
                .SetMaterial("Transition.3")
                .BeginAsync(LMotion.Create(1.0f, 0.0f, 0.5f));
            await areaTitleAnimation.PlayAsync("In", cancellationToken);
            await areaPressButtonAnimation.PlayAsync("In", cancellationToken);
            areaPressButtonAnimation.Play("Loop");
            await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(cancellationToken);
            gameCameraController.EndTitle();
            await UniTask.WhenAll(
                areaTitleAnimation.PlayAsync("Out", cancellationToken),
                areaPressButtonAnimation.PlayAsync("Out", cancellationToken)
            );
            gameEvents.OnEndTitle.OnNext(Unit.Default);
            inputController.PopActionType();
            document.DestroySafe();
        }
    }
}
