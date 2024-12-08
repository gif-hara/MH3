using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponViewCritical : Sequence
    {
        [SerializeField]
        private string instanceWeaponDataKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceWeaponData = container.Resolve<InstanceWeapon>(instanceWeaponDataKey);
            text.text = instanceWeaponData.Critical.ToString("0%");
            return UniTask.CompletedTask;
        }
    }
}
