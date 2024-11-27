using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HK
{
    /// <summary>
    /// Represents a tiny state machine that allows changing states asynchronously.
    /// </summary>
    public sealed class TinyStateMachine : IDisposable
    {
        private CancellationTokenSource scope = null;

        private bool isDisposed = false;

        /// <summary>
        /// Event that is triggered when the state begins.
        /// </summary>
        public event Action<Func<CancellationToken, UniTask>> OnBeginState;
        
        /// <summary>
        /// Event that is triggered when the state ends.
        /// </summary>
        public event Action<Func<CancellationToken, UniTask>> OnEndState;

        ~TinyStateMachine()
        {
            Dispose();
        }

        /// <summary>
        /// Changes the state asynchronously.
        /// </summary>
        /// <param name="state">The state to change to.</param>
        /// <returns>A <see cref="UniTask"/> representing the asynchronous operation.</returns>
        private async UniTask ChangeAsync(Func<CancellationToken, UniTask> state)
        {
            if (isDisposed)
            {
                return;
            }

            Clear();
            scope = new CancellationTokenSource();
            try
            {
                // Wait for one frame to ensure that the previous state has completed before starting the next state.
                await UniTask.NextFrame(scope.Token);
                OnBeginState?.Invoke(state);
                await state(scope.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                OnEndState?.Invoke(state);
                if (isDisposed)
                {
                    OnEndState = null;
                }
            }
        }

        /// <summary>
        /// Changes the state synchronously.
        /// </summary>
        /// <param name="state">The state to change to.</param>
        public void Change(Func<CancellationToken, UniTask> state)
        {
            ChangeAsync(state).Forget();
        }
        
        /// <summary>
        /// Disposes the state machine and cancels any ongoing state change.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Clear();
            OnBeginState = null;
            isDisposed = true;
        }

        /// <summary>
        /// Cancels any ongoing state change and clears the state machine.
        /// </summary>
        public void Clear()
        {
            scope?.Cancel();
            scope?.Dispose();
            scope = null;
        }
    }
}