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
        
        public UIViewListElementBuilder InvokeSequence(SequenceNames name)
        {
            var sequenceName = name switch
            {
                SequenceNames.Default => "Default",
                SequenceNames.Deactive => "Deactive",
                SequenceNames.Primary => "Primary",
                _ => throw new ArgumentOutOfRangeException($"Invalid name: {name}")
            };
            elementDocument
                .Q<HKUIDocument>("Sequences")
                .Q<SequencesMonoBehaviour>(sequenceName)
                .PlayAsync(new Container(), elementDocument.destroyCancellationToken)
                .Forget();
            return this;
        }
        
        public enum SequenceNames
        {
            Default,
            Deactive,
            Primary
        }
    }
}
