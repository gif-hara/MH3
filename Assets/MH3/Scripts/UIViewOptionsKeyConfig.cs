using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
using UnityEngine;

namespace MH3
{
    public class UIViewOptionsKeyConfig : IUIViewOptions
    {
        private readonly UIViewList list;

        public UIViewOptionsKeyConfig(HKUIDocument listDocumentPrefab)
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            list = UIViewList.CreateWithPages(
                listDocumentPrefab,
                gameRules.KeyConfigElements.Select(x => new Action<HKUIDocument>(document =>
                {
                    var builder = new UIViewListElementBuilder(document);
                    builder.EditHeader(header => header.text = x.InputName);
                    builder.EditButton(button =>
                    {
                        button.OnClickAsObservable()
                            .Subscribe(_ =>
                            {
                                Debug.Log("Clicked");
                            })
                            .RegisterTo(button.destroyCancellationToken);
                    });
                    document.Q<TMP_Text>("InputSprite").text = InputSprite.GetTag(x.InputActionReference.action);
                })),
                    0,
                false
                );
        }

        public async UniTask ActivateAsync(CancellationToken scope)
        {
            list.SetSelectable(0);
            var inputController = TinyServiceLocator.Resolve<InputController>();
            await inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .FirstAsync(scope);
        }

        public void Dispose()
        {
            list.Dispose();
        }
    }
}
