using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class OpenUIViewAcquireRewardAsync : Sequence
    {
        [SerializeField]
        private HKUIDocument documentPrefab;

        [SerializeField]
        private string instanceWeaponsKey;

        [SerializeField]
        private string instanceSkillCoresKey;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            return UIViewAcquireReward.OpenAsync(
                documentPrefab,
                container.Resolve<List<InstanceWeapon>>(instanceWeaponsKey),
                container.Resolve<List<InstanceSkillCore>>(instanceSkillCoresKey),
                cancellationToken
                );
        }
    }
}
