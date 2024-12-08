using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using static UnityEngine.InputSystem.InputAction;

namespace MH3
{
    public class UIViewSelectInstanceSkillCore
    {
        public static UniTask OpenAsync(
            HKUIDocument listDocumentPrefab,
            Action<InstanceSkillCore> onClickAction,
            Action<CallbackContext> onCancelAction,
            CancellationToken scope
            )
        {
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var list = UIViewList.CreateWithPages(
                listDocumentPrefab,
                TinyServiceLocator.Resolve<UserData>().InstanceSkillCoreList
                    .Select(x => new Action<HKUIDocument>(document =>
                    {
                        UIViewList.ApplyAsSimpleElement(document, x.SkillCoreSpec.Name, _ =>
                        {
                            onClickAction(x);
                        },
                        _ =>
                        {
                        });
                    })),
                0
            );
            inputController.Actions.UI.Cancel
                .OnPerformedAsObservable()
                .Subscribe(onCancelAction)
                .RegisterTo(scope);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                list.DestroySafe();
                inputController.PopActionType();
            });
            return UniTask.WaitUntilCanceled(scope);
        }
    }
}
