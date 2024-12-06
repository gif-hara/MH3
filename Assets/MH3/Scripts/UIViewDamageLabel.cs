using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;

namespace MH3
{
    public class UIViewDamageLabel : UIView
    {
        private readonly HKUIDocument document;

        private readonly Camera camera;

        public UIViewDamageLabel(HKUIDocument documentPrefab, Camera camera, CancellationToken scope)
            : base(scope)
        {
            document = Object.Instantiate(documentPrefab);
            this.camera = camera;
        }

        public void BeginObserve(Actor actor)
        {
            actor.SpecController.OnTakeDamage
                .Subscribe(this, static (x, t) =>
                {
                    t.CreateLabelAsync(x.Damage, x.DamagePosition).Forget();
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        private async UniTask CreateLabelAsync(int damage, Vector3 worldPosition)
        {
            var label = Object.Instantiate(
                document.Q<HKUIDocument>("Prefab.Element"),
                document.Q<RectTransform>("Parent.Element"),
                false
                );
            label.Q<TMP_Text>("Label").text = damage.ToString();
            label.UpdateAsObservable()
                .Subscribe((label, camera, worldPosition), static (_, t) =>
                {
                    var (label, camera, worldPosition) = t;
                    label.transform.position = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
                })
                .RegisterTo(label.destroyCancellationToken);
            await label.Q<SimpleAnimation>("Animation").PlayAsync("Default", document.destroyCancellationToken);
            Object.Destroy(label.gameObject);
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
