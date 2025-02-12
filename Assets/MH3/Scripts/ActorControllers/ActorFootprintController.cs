using System;
using HK;
using R3;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorFootprintController : MonoBehaviour
    {
        [SerializeField]
        private string effectKey;

        [SerializeField]
        private float instantiateInterval;

        [SerializeField]
        private Vector2 randomPosition;

        void Start()
        {
            if (string.IsNullOrEmpty(effectKey))
            {
                return;
            }
            Observable.Interval(TimeSpan.FromSeconds(instantiateInterval))
                .Subscribe(_ =>
                {
                    var effect = TinyServiceLocator.Resolve<EffectManager>().Rent(effectKey);
                    effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-randomPosition.x, randomPosition.x), 0.0f, UnityEngine.Random.Range(-randomPosition.y, randomPosition.y));
                    effect.transform.rotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);
                })
                .RegisterTo(destroyCancellationToken);
        }
    }
}
