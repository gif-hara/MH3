using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class PlayLitMotionAnimation : Sequence
    {
        [SerializeField]
        private LitMotionAnimation litMotionAnimation;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            litMotionAnimation.Restart();
            return UniTask.CompletedTask;
        }
    }
}
