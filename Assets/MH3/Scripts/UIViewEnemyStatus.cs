using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using TMPro;
using UnityEngine.UI;

namespace MH3
{
    public class UIViewEnemyStatus
    {
        private readonly HKUIDocument document;

        public UIViewEnemyStatus(HKUIDocument documentPrefab, CancellationToken scope)
        {
            document = UnityEngine.Object.Instantiate(documentPrefab);
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
        }

        public void BeginObserve(Actor actor)
        {
            document.gameObject.SetActive(actor.SpecController.VisibleStatusUI);
            actor.SpecController.HitPoint
                .Subscribe(document, (_, d) =>
                {
                    d.Q<HKUIDocument>("Slider.HitPoint")
                        .Q<Slider>("Slider")
                        .value = (float)actor.SpecController.HitPoint.CurrentValue / actor.SpecController.HitPointMaxTotal;
                })
                .RegisterTo(actor.destroyCancellationToken);
            document.Q<TMP_Text>("Name").text = actor.SpecController.ActorName;
        }
    }
}
