using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine.UI;

namespace MH3
{
    public class UIViewPlayerStatus : UIView
    {
        private readonly HKUIDocument document;

        public UIViewPlayerStatus(HKUIDocument documentPrefab, Actor actor, CancellationToken scope)
            : base(scope)
        {
            document = UnityEngine.Object.Instantiate(documentPrefab);
            actor.SpecController.HitPoint
                .Subscribe(document, (_, d) =>
                {
                    d.Q<HKUIDocument>("Slider.HitPoint")
                        .Q<Slider>("Slider")
                        .value = (float)actor.SpecController.HitPoint.CurrentValue / actor.SpecController.HitPointMax.CurrentValue;
                })
                .RegisterTo(scope);
        }

        protected override void OnDispose()
        {
            if (document != null)
            {
                UnityEngine.Object.Destroy(document.gameObject);
            }
        }
    }
}
