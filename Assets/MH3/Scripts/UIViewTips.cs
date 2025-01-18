using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using UnityEngine;

namespace MH3
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UIViewTips
    {
        private readonly HKUIDocument document;

        private readonly TMP_Text message;

        private readonly RectTransform messageRectTransform;

        private readonly RectTransform messageParent;

        private readonly CanvasGroup canvasGroup;

        private CancellationTokenSource animationScope = null;

        public UIViewTips(HKUIDocument documentPrefab, CancellationToken scope)
        {
            document = UnityEngine.Object.Instantiate(documentPrefab);
            message = document.Q<TMP_Text>("Message");
            messageRectTransform = message.GetComponent<RectTransform>();
            messageParent = message.transform.parent.GetComponent<RectTransform>();
            canvasGroup = document.Q<CanvasGroup>("CanvasGroup");
            document.gameObject.SetActive(false);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                if (document == null)
                {
                    return;
                }
                UnityEngine.Object.Destroy(document.gameObject);
            });
        }

        public static void Close()
        {
            var instance = TinyServiceLocator.Resolve<UIViewTips>();
            instance.document.gameObject.SetActive(false);
            instance.animationScope?.Cancel();
            instance.animationScope?.Dispose();
            instance.animationScope = null;
        }

        public static void SetTip(string tip)
        {
            var instance = TinyServiceLocator.Resolve<UIViewTips>();
            instance.UpdateTextAsync(tip);
        }

        private void UpdateTextAsync(string tip)
        {
            document.gameObject.SetActive(true);
            message.text = tip;
            messageRectTransform.anchoredPosition = Vector2.zero;
            messageRectTransform.sizeDelta = new Vector2(message.preferredWidth, messageParent.sizeDelta.y);
            animationScope?.Cancel();
            animationScope?.Dispose();
            animationScope = new CancellationTokenSource();
            if (message.preferredWidth > messageParent.rect.width)
            {
                BeginSlideAnimationAsync();
            }
        }

        private async void BeginSlideAnimationAsync()
        {
            try
            {
                while (true)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(3.0f), cancellationToken: animationScope.Token);
                    await LMotion.Create(0.0f, -message.preferredWidth, message.preferredWidth / 100.0f)
                        .BindToAnchoredPositionX(messageRectTransform)
                        .ToUniTask(animationScope.Token);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: animationScope.Token);
                    await UniTask.WhenAll
                    (
                        LMotion.Create(0.0f, 1.0f, 0.3f)
                            .BindToCanvasGroupAlpha(canvasGroup)
                            .ToUniTask(animationScope.Token),
                        LMotion.Create(20.0f, 0.0f, 0.3f)
                            .BindToAnchoredPositionX(messageRectTransform)
                            .ToUniTask(animationScope.Token)
                    );
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}