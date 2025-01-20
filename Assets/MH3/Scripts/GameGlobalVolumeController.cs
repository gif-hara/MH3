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

        [SerializeField]
        private Volume superArmorVolume;

        [SerializeField]
        private float superArmorDuration;

        private CancellationDisposable damageAnimationScope = null;

        public void BeginObserves(Actor player)
        {
            player.SpecController.OnTakeDamage
                .Subscribe(this, (x, t) =>
                {
                    if (x.GuardResult == Define.GuardResult.NotGuard || x.GuardResult == Define.GuardResult.FailedGuard)
                    {
                        t.damageAnimationScope?.Dispose();
                        t.damageAnimationScope = new CancellationDisposable();
                        if (x.ConsumedSuperArmor)
                        {
                            PlayVolumeAnimationAsync(superArmorVolume, superArmorDuration, t.damageAnimationScope)
                                .Forget();
                        }
                        else
                        {
                            PlayVolumeAnimationAsync(takeDamageVolume, takeDamageDuration, t.damageAnimationScope)
                                .Forget();
                        }
                    }
                });
        }

        private UniTask PlayVolumeAnimationAsync(Volume volume, float duration, CancellationDisposable scope)
        {
            scope.Token.RegisterWithoutCaptureExecutionContext(() => volume.weight = 0.0f);
            return LMotion.Create(1.0f, 0.0f, duration)
                .Bind(x => volume.weight = x)
                .ToUniTask(cancellationToken: scope.Token);
        }
    }
}
