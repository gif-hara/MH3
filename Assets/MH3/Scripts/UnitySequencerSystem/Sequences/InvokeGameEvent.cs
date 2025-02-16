using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InvokeGameEvent : Sequence
    {
        [SerializeField]
        private GameEvents.Type gameEventType;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<GameEvents>().GetSubject(gameEventType).OnNext(Unit.Default);
            return UniTask.CompletedTask;
        }
    }
}
