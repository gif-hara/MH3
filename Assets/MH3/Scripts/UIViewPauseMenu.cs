using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using TMPro;

namespace MH3
{
    public class UIViewPauseMenu
    {
        public static async UniTask OpenAsync(HKUIDocument headerDocumentPrefab, HKUIDocument listDocumentPrefab, Actor actor, CancellationToken scope)
        {
            var homeMenuScope = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(scope));
            var header = UnityEngine.Object.Instantiate(headerDocumentPrefab);
            var stateMachine = new TinyStateMachine();

            // 待機
            {
                await UniTask.WaitUntilCanceled(homeMenuScope.Token);
            }

            // 終了処理
            {
                homeMenuScope.Dispose();
                stateMachine.Dispose();
            }

            void SetHeaderText(string text)
            {
                header.Q<TMP_Text>("Header").text = text;
            }
        }
    }
}
