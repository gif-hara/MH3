using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MH3
{
    public class UIViewOptionsKeyConfig : IUIViewOptions
    {
        private readonly UIViewList list;
        
        private readonly HKUIDocument rebindingDocument;

        public UIViewOptionsKeyConfig(HKUIDocument listDocumentPrefab, HKUIDocument rebindingDocumentPrefab)
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            list = UIViewList.CreateWithPages(
                listDocumentPrefab,
                gameRules.KeyConfigElements.Select(x => new Action<HKUIDocument>(document =>
                {
                    var builder = new UIViewListElementBuilder(document);
                    builder.EditHeader(header => header.text = x.InputName.Localized());
                    builder.EditButton(button =>
                    {
                        button.OnClickAsObservable()
                            .Subscribe(_ =>
                            {
                                BeginRebindingAsync(x.InputActionReference.action, x.SaveKey, button).Forget();
                            })
                            .RegisterTo(button.destroyCancellationToken);
                    });
                    document.Q<TMP_Text>("InputSprite").text = InputSprite.GetTag(x.InputActionReference.action);
                })),
                    0,
                false
                );
            rebindingDocument = Object.Instantiate(rebindingDocumentPrefab);
            rebindingDocument.gameObject.SetActive(false);
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
        
        private async UniTask BeginRebindingAsync(InputAction inputAction, string saveKey, Selectable listElementSelectable)
        {
            EventSystem.current.SetSelectedGameObject(null);
            rebindingDocument.gameObject.SetActive(true);
            var inputController = TinyServiceLocator.Resolve<InputController>();
            await inputController.BeginRebindingAsync(inputAction);
            rebindingDocument.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(listElementSelectable.gameObject);
            var saveData = TinyServiceLocator.Resolve<SaveData>();
            saveData.KeyConfigData.AddOrReplace(saveKey, inputAction.SaveBindingOverridesAsJson());
            SaveSystem.Save(saveData, SaveData.Path);
        }
    }
}
