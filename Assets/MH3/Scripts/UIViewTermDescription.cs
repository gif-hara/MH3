using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
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
            var descriptionDocument = UnityEngine.Object.Instantiate(descriptionDocumentPrefab);
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
                            descriptionDocument.Q<TMP_Text>("Text.Title").text = element.Title;
                            descriptionDocument.Q<TMP_Text>("Text.Description").text = element.Description;
                        });
                })),
                0
            );
            await TinyServiceLocator.Resolve<InputController>().Actions.UI.Cancel
                .OnPerformedAsObservable()
                .FirstAsync(cancellationToken);
            descriptionDocument.DestroySafe();
            listDocument.DestroySafe();
        }

        public class Element
        {
            public string Title { get; }

            public string Description { get; }

            public Element(string title, string description)
            {
                Title = title;
                Description = description;
            }
        }
    }
}
