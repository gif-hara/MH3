using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceSkillCoreViewName : Sequence
    {
        [SerializeField]
        private string instanceSkillCoreKey;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceSkillCore = container.Resolve<InstanceSkillCore>(instanceSkillCoreKey);
            text.text = instanceSkillCore.SkillCoreSpec.LocalizedName;
            return UniTask.CompletedTask;
        }
    }
}
