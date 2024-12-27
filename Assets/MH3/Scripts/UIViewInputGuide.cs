using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;

namespace MH3
{
    public class UIViewInputGuide
    {
        private readonly HKUIDocument document;

        private readonly Stack<Func<string>> textSelectors = new();

        private CancellationDisposable inputSchemeScope = null;

        public UIViewInputGuide(HKUIDocument documentPrefab, CancellationToken scope)
        {
            document = UnityEngine.Object.Instantiate(documentPrefab);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
            document.gameObject.SetActive(false);
        }

        public void Push(Func<string> textSelector, CancellationToken scope)
        {
            document.gameObject.SetActive(true);
            textSelectors.Push(textSelector);
            if (inputSchemeScope == null)
            {
                inputSchemeScope = new CancellationDisposable();
                TinyServiceLocator.Resolve<InputScheme>().AnyChangedAsObservable()
                .Subscribe(_ =>
                {
                    document.Q<TMP_Text>("Text").text = textSelector();
                })
                .RegisterTo(inputSchemeScope.Token);
            }
            else
            {
                document.Q<TMP_Text>("Text").text = textSelector();
            }
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                textSelectors.Pop();
                if (textSelectors.Count == 0)
                {
                    inputSchemeScope.Dispose();
                    inputSchemeScope = null;
                    document.gameObject.SetActive(false);
                }
                else
                {
                    document.Q<TMP_Text>("Text").text = textSelectors.Peek()();
                }
            });
        }
    }
}
