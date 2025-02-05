using System;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine.EventSystems;
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
        
        public UIViewListElementBuilder InvokeSequence(string sequenceName)
        {
            elementDocument
                .Q<HKUIDocument>("Sequences")
                .Q<SequencesMonoBehaviour>(sequenceName)
                .PlayAsync(new Container(), elementDocument.destroyCancellationToken)
                .Forget();
            return this;
        }

        public UIViewListElementBuilder InvokeSequenceDefault()
        {
            return InvokeSequence("Default");
        }
            
        public UIViewListElementBuilder InvokeSequenceDeactive()
        {
            return InvokeSequence("Deactive");
        }
        
        public UIViewListElementBuilder InvokeSequencePrimary()
        {
            return InvokeSequence("Primary");
        }
    }
}
