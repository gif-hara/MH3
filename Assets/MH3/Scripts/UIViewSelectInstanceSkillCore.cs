using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnitySequencerSystem;
using static UnityEngine.InputSystem.InputAction;

namespace MH3
{
    public class UIViewSelectInstanceSkillCore
    {
        public static UniTask OpenAsync(
            HKUIDocument listDocumentPrefab,
            HKUIDocument instanceSkillCoreViewDocumentPrefab,
            IEnumerable<InstanceSkillCore> instanceSkillCores,
            Action<InstanceSkillCore> onClickAction,
            CancellationToken scope
            )
        {
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var instanceSkillCoreView = UnityEngine.Object.Instantiate(instanceSkillCoreViewDocumentPrefab);
            var instanceSkillCoreSequences = instanceSkillCoreView.Q<SequencesMonoBehaviour>("Sequences");
            var list = UIViewList.CreateWithPages(
                listDocumentPrefab,
                instanceSkillCores
                    .Select(x => new Action<HKUIDocument>(document =>
                    {
                        UIViewList.ApplyAsSimpleElement(document, x.SkillCoreSpec.Name, _ =>
                        {
                            onClickAction(x);
                        },
                        _ =>
                        {
                            var container = new Container();
                            container.Register("InstanceSkillCore", x);
                            instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                        });
                    })),
                0
            );
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                instanceSkillCoreView.DestroySafe();
                list.DestroySafe();
                inputController.PopActionType();
            });
            return UniTask.WaitUntilCanceled(scope);
        }
    }
}
