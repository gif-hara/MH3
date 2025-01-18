using Cysharp.Threading.Tasks;
using LitMotion;
using MH3.ActorControllers;
using R3;
using UnityEngine;
using UnityEngine.Rendering;

namespace MH3
{
    public class GameGlobalVolumeController : MonoBehaviour
    {
        [SerializeField]
        private Volume takeDamageVolume;

        [SerializeField]
        private float takeDamageDuration;

        private CancellationDisposable takeDamageAnimationScope = null;

        public void BeginObserves(Actor player)
        {
            player.SpecController.OnTakeDamage
                .Subscribe(this, (_, t) =>
                {
                    t.takeDamageAnimationScope?.Dispose();
                    t.takeDamageAnimationScope = new CancellationDisposable();
                    LMotion.Create(1.0f, 0.0f, t.takeDamageDuration)
                        .Bind(x => takeDamageVolume.weight = x)
                        .ToUniTask(cancellationToken: t.takeDamageAnimationScope.Token);
                });
        }
    }
}
