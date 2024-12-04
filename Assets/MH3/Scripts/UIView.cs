using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MH3
{
    public abstract class UIView : IDisposable
    {
        public UIView(CancellationToken scope)
        {
            scope.RegisterWithoutCaptureExecutionContext(() => Dispose());
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected abstract void OnDispose();
    }
}
