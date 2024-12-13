using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponViewAbnormalStatus : Sequence
    {
        [SerializeField]
        private string instanceWeaponKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceWeapon = container.Resolve<InstanceWeapon>(instanceWeaponKey);
            if (instanceWeapon.AbnormalStatusType == Define.AbnormalStatusType.None)
            {
                text.text = "なし";
            }
            else
            {
                text.text = $"[{instanceWeapon.AbnormalStatusType.GetName()}] {instanceWeapon.AbnormalStatusAttack}";
            }
            return UniTask.CompletedTask;
        }
    }
}
