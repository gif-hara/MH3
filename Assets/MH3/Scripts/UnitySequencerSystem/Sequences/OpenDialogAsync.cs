using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class OpenDialogAsync : Sequence
    {
        [SerializeField]
        private HKUIDocument documentPrefab;

        [SerializeReference, SubclassSelector]
        private StringResolver messageResolver;

        [SerializeField]
        private List<string> options;

        [SerializeReference, SubclassSelector]
        private IntResolver initialSelectionIndexResolver;

        [SerializeReference, SubclassSelector]
        private IntResolver cancelIndexResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver resultResolver;

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var dialog = new UIViewSimpleDialog(documentPrefab);
            var result = await dialog.OpenAsync(
                messageResolver.Resolve(container).Localized(),
                options.Select(x => x.Localized()),
                initialSelectionIndexResolver.Resolve(container),
                cancelIndexResolver.Resolve(container),
                cancellationToken
                );
            if (resultResolver != null)
            {
                container.RegisterOrReplace(resultResolver.Resolve(container), result);
            }
        }
    }
}
