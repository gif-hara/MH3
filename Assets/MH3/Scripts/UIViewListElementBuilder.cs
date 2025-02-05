using System;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine.UI;
using UnitySequencerSystem;

namespace MH3
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewListElementBuilder
    {
        private readonly HKUIDocument elementDocument;

        public UIViewListElementBuilder(HKUIDocument elementDocument)
        {
            this.elementDocument = elementDocument;
        }

        public UIViewListElementBuilder EditHeader(Action<TMP_Text> action)
        {
            action(elementDocument.Q<TMP_Text>("Header"));
            return this;
        }

        public UIViewListElementBuilder EditButton(Action<Button> action)
        {
            action(elementDocument.Q<Button>("Button"));
            return this;
        }

        public UIViewListElementBuilder EditButton(Action<Button, UIViewListElementBuilder> action)
        {
            action(elementDocument.Q<Button>("Button"), this);
            return this;
        }

        public UIViewListElementBuilder ApplyStyle(StyleNames name)
        {
            var sequenceName = name switch
            {
                StyleNames.Default => "Default",
                StyleNames.Deactive => "Deactive",
                StyleNames.Primary => "Primary",
                _ => throw new ArgumentOutOfRangeException($"Invalid name: {name}")
            };
            elementDocument
                .Q<HKUIDocument>("StyleSequences")
                .Q<SequencesMonoBehaviour>(sequenceName)
                .PlayAsync(new Container(), elementDocument.destroyCancellationToken)
                .Forget();
            return this;
        }

        public UIViewListElementBuilder SetActiveBadge(bool active)
        {
            elementDocument.Q("Area.Badge").SetActive(active);
            return this;
        }

        public enum StyleNames
        {
            Default,
            Deactive,
            Primary
        }
    }
}
