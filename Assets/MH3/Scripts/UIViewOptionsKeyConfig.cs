using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace MH3
{
    public class UIViewOptionsKeyConfig
    {
        private HKUIDocument document;


        public static UIViewOptionsKeyConfig Open(
            HKUIDocument documentPrefab,
            CancellationToken scope
            )
        {
            var instance = new UIViewOptionsKeyConfig();
            instance.Initialize(documentPrefab);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                instance.Close();
            });
            return instance;
        }

        private void Initialize(HKUIDocument documentPrefab)
        {
            document = Object.Instantiate(documentPrefab);
        }

        private void Close()
        {
            document.DestroySafe();
        }
    }
}
