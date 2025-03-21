using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;

namespace MH3
{
    public class UIViewBeginQuest
    {
        public static UniTask OpenAsync(HKUIDocument documentPrefab, string enemyName, CancellationToken scope)
        {
            var document = UnityEngine.Object.Instantiate(documentPrefab);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
            document.Q<TMP_Text>("EnemyName").text = enemyName;
            var animation = document.Q<SimpleAnimation>("Animation");
            const string animationName = "Default";
            if (animation.IsPlaying(animationName))
            {
                animation.Stop(animationName);
            }
            return animation.PlayAsync(animationName, scope);
        }
    }
}
