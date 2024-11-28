using System;
using System.Threading;
using R3;
using UnityEngine;

namespace MH3
{
    public class EarlyInputHandler
    {
        public static IDisposable Invoke(Func<bool> process, float inputTime, CancellationToken scope)
        {
            if (process())
            {
                return Disposable.Empty;
            }

            var newScopeSource = CancellationTokenSource.CreateLinkedTokenSource(scope);
            return Observable.EveryUpdate(scope)
                .TakeWhile(_ => inputTime > 0)
                .Subscribe((process, inputTime, newScopeSource), static (_, t) =>
                {
                    t.inputTime -= Time.deltaTime;
                    if (t.process())
                    {
                        t.newScopeSource.Cancel();
                        t.newScopeSource.Dispose();
                    }
                })
                .RegisterTo(newScopeSource.Token);
        }
    }
}
