using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MH3
{
    public interface IUIViewOptions : IDisposable
    {
        UniTask ActivateAsync(CancellationToken scope);
    }
}
