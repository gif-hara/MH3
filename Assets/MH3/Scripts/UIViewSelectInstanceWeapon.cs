using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnitySequencerSystem;
using static UnityEngine.InputSystem.InputAction;

namespace MH3
{
    public class UIViewSelectInstanceWeapon
    {
        public static UniTask OpenAsync(
            HKUIDocument listDocumentPrefab,
            HKUIDocument instanceWeaponViewDocumentPrefab,
            Action<InstanceWeapon> selectInstanceWeaponOnClickAction,
            Action<CallbackContext> selectInstanceWeaponOnCancelAction,
            CancellationToken scope
            )
        {
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var instanceWeaponView = UnityEngine.Object.Instantiate(instanceWeaponViewDocumentPrefab);
            var instanceWeaponSequences = instanceWeaponView.Q<SequencesMonoBehaviour>("Sequences");
            var list = UIViewList.CreateWithPages(
                listDocumentPrefab,
                TinyServiceLocator.Resolve<UserData>().InstanceWeapons
                    .Select(x => new Action<HKUIDocument>(document =>
                    {
                        UIViewList.ApplyAsSimpleElement(document, x.WeaponSpec.Name, _ =>
                        {
                            selectInstanceWeaponOnClickAction(x);
                        },
                        _ =>
                        {
                            var container = new Container();
                            container.Register("InstanceWeapon", x);
                            instanceWeaponSequences.PlayAsync(container, scope).Forget();
                        });
                    })),
                0
            );
            inputController.Actions.UI.Cancel
                .OnPerformedAsObservable()
                .Subscribe(selectInstanceWeaponOnCancelAction)
                .RegisterTo(scope);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                list.DestroySafe();
                instanceWeaponView.DestroySafe();
                inputController.PopActionType();
            });
            return UniTask.WaitUntilCanceled(scope);
        }
    }
}
