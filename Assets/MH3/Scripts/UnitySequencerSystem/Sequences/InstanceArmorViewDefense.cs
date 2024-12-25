using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceArmorViewDefense : Sequence
    {
        [SerializeField]
        private string instanceArmorKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceArmor = container.Resolve<InstanceArmor>(instanceArmorKey);
            text.text = instanceArmor == null ? "0" : instanceArmor.Defense.ToString();
            return UniTask.CompletedTask;
        }
    }
}
