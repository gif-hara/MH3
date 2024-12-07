using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;

namespace MH3
{
    public class UIViewDebugMenu
    {
        public static async UniTask OpenAsync(HKUIDocument listDocumentPrefab, CancellationToken scope)
        {
            var tempTimeScale = HK.Time.Root.timeScale;
            HK.Time.Root.timeScale = 0.0f;
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);
            var debugMenuScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
            var list = UIViewList.CreateWithPages(
                listDocumentPrefab,
                new List<Action<HKUIDocument>>
                {
                    document =>
                    {
                        UIViewList.ApplyAsSimpleElement(document, "Debug1", _ => Debug.Log("Debug1"));
                    },
                    document =>
                    {
                        UIViewList.ApplyAsSimpleElement(document, "Debug2", _ => Debug.Log("Debug2"));
                    },
                    document =>
                    {
                        UIViewList.ApplyAsSimpleElement(document, "Debug3", _ => Debug.Log("Debug3"));
                    },
                },
                0
                );
            await UniTask.WaitUntilCanceled(debugMenuScope.Token);
            if (list != null)
            {
                UnityEngine.Object.Destroy(list.gameObject);
            }
            inputController.PopActionType();
            HK.Time.Root.timeScale = tempTimeScale;
        }
    }
}
