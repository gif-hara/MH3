using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace MH3
{
    public class UIViewTermDescription
    {
        public static async UniTask OpenAsync(
            HKUIDocument listDocumentPrefab,
            HKUIDocument descriptionDocumentPrefab,
            IEnumerable<Element> elements,
            CancellationToken cancellationToken = default
        )
        {
            var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var inputController = TinyServiceLocator.Resolve<InputController>();
            var uiViewInputGuide = TinyServiceLocator.Resolve<UIViewInputGuide>();
            var descriptionDocument = UnityEngine.Object.Instantiate(descriptionDocumentPrefab);
            var pageIndex = 0;
            Element currentElement = null;
            var listDocument = UIViewList.CreateWithPages(
                listDocumentPrefab,
                elements.Select(element => new Action<HKUIDocument>(document =>
                {
                    UIViewList.ApplyAsSimpleElement(
                        document,
                        element.Title,
                        _ => { },
                        _ =>
                        {
                            pageIndex = 0;
                            currentElement = element;
                            UpdateDescription(pageIndex);
                        });
                })),
                0
            );
            inputController.Actions.UI.PagePrevious
                .OnPerformedAsObservable()
                .Subscribe(_ =>
                {
                    pageIndex = Mathf.Max(0, pageIndex - 1);
                    UpdateDescription(pageIndex);
                })
                .AddTo(scope.Token);
            inputController.Actions.UI.PageNext
                .OnPerformedAsObservable()
                .Subscribe(_ =>
                {
                    pageIndex = Mathf.Min(currentElement.Pages.Count - 1, pageIndex + 1);
                    UpdateDescription(pageIndex);
                })
                .AddTo(scope.Token);
            void UpdateDescription(int pageIndex)
            {
                var page = currentElement.Pages[pageIndex];
                descriptionDocument.Q<TMP_Text>("Text.Title").text = page.Title;
                var description = page.Description;
                description = description.Replace("{InputSprite.Player.Guard}", InputSprite.GetTag(inputController.Actions.Player.Guard));
                descriptionDocument.Q<TMP_Text>("Text.Description").text = description;
            }
            uiViewInputGuide.Push(() => string.Format(
                "{0}:選択 {1}:戻る {2}:前のページ {3}:次のページ",
                InputSprite.GetTag(inputController.Actions.UI.Navigate),
                InputSprite.GetTag(inputController.Actions.UI.Cancel),
                InputSprite.GetTag(inputController.Actions.UI.PagePrevious),
                InputSprite.GetTag(inputController.Actions.UI.PageNext)
                ).Localized(), scope.Token);
            await TinyServiceLocator.Resolve<InputController>().Actions.UI.Cancel
                .OnPerformedAsObservable()
                .FirstAsync(scope.Token);
            descriptionDocument.DestroySafe();
            listDocument.DestroySafe();
            scope.Cancel();
            scope.Dispose();
        }

        public class Element
        {
            public string Title { get; }

            public List<Page> Pages { get; }

            public Element(string title, List<Page> pages)
            {
                Title = title;
                Pages = pages;
            }

            public Element(MasterData.TermDescriptionSpec spec)
            {
                Title = spec.Title;
                Pages = spec.GetPages().Select(p => new Page(p.Title, p.Description)).ToList();
            }
        }

        public class Page
        {
            public string Title { get; }

            public string Description { get; }

            public Page(string title, string description)
            {
                Title = title;
                Description = description;
            }
        }
    }
}
