using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponViewWeaponName : Sequence
    {
        [SerializeField]
        private string instanceWeaponDataKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceWeaponData = container.Resolve<InstanceWeapon>(instanceWeaponDataKey);
            text.text = instanceWeaponData.WeaponSpec.LocalizedName;
            return UniTask.CompletedTask;
        }
    }
}
