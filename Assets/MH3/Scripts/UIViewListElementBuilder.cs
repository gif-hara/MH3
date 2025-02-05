using System;
using HK;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
            
        public UIViewListElementBuilder SetOnClick(Action<Unit> onClick)
        {
            elementDocument.Q<Button>("Button").OnClickAsObservable()
                .Subscribe(onClick)
                .RegisterTo(elementDocument.destroyCancellationToken);
            return this;
        }
            
        public UIViewListElementBuilder SetOnSelect(Action<BaseEventData> onSelect)
        {
            elementDocument.Q<Button>("Button").OnSelectAsObservable()
                .Subscribe(onSelect)
                .RegisterTo(elementDocument.destroyCancellationToken);
            return this;
        }
            
        public UIViewListElementBuilder SetOnDeselect(Action<BaseEventData> onDeselect)
        {
            elementDocument.Q<Button>("Button").OnDeselectAsObservable()
                .Subscribe(onDeselect)
                .RegisterTo(elementDocument.destroyCancellationToken);
            return this;
        }
    }
}
