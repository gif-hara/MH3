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
            if (actor.SpecController.VisibleStatusUI)
            {
                var gameEvents = TinyServiceLocator.Resolve<GameEvents>();
                Observable.Merge
                    (
                        gameEvents.OnBeginPauseMenu,
                        gameEvents.OnBeginAcquireReward,
                        gameEvents.OnBeginBattleStartEffect
                    )
                    .Subscribe(document, static (_, d) =>
                    {
                        d.gameObject.SetActive(false);
                    })
                    .RegisterTo(actor.destroyCancellationToken);
                Observable.Merge
                    (
                        gameEvents.OnEndPauseMenu,
                        gameEvents.OnEndAcquireReward
                    )
                    .Subscribe(document, static (_, d) =>
                    {
                        d.gameObject.SetActive(true);
                    })
                    .RegisterTo(actor.destroyCancellationToken);
                gameEvents.OnEndBattleStartEffect
                    .Subscribe(document, static (_, d) =>
                    {
                        d.gameObject.SetActive(true);
                        d.Q<SimpleAnimation>("Area.Animation").Play("In");
                    })
                    .RegisterTo(actor.destroyCancellationToken);
            }
            document.Q<TMP_Text>("Name").text = actor.SpecController.ActorName;
        }
    }
}
