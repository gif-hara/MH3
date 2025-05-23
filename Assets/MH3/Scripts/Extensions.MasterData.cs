using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    public static partial class Extensions
    {
        public static MasterData.SkillLevelValue GetFromLevel(this MasterData.SkillLevelValue.DictionaryList self, int level)
        {
            return self.Get(self.GetFixedSkillLevel(level));
        }

        public static int GetFixedSkillLevel(this MasterData.SkillLevelValue.DictionaryList self, int level)
        {
            return Mathf.Clamp(level, 0, self.List.Count - 1);
        }

        public static List<MasterData.WeaponCombo> GetWeaponCombos(this MasterData.SpearCombo self)
        {
            return TinyServiceLocator.Resolve<MasterData>().WeaponCombos.Get(self.WeaponComboId);
        }

        public static MasterData.SpearSpec GetSpearSpec(this MasterData.WeaponSpec self)
        {
            return TinyServiceLocator.Resolve<MasterData>().SpearSpecs.Get(self.Id);
        }

        public static List<MasterData.SpearCombo> GetSpearCombos(this MasterData.SpearSpec self)
        {
            return TinyServiceLocator.Resolve<MasterData>().SpearCombos.Get(self.SpearComboId);
        }

        public static List<MasterData.TermDescriptionPage> GetPages(this MasterData.TermDescriptionSpec self)
        {
            return TinyServiceLocator.Resolve<MasterData>().TermDescriptionPages.Get(self.Id);
        }

        public static async UniTask PlayAsync(this List<MasterData.AvailableContentsEvent> self, CancellationToken cancellationToken)
        {
            var userData = TinyServiceLocator.Resolve<UserData>();
            foreach (var availableContentsEvent in self)
            {
                if (userData.AvailableContents.NewContents.Contains(availableContentsEvent.NewContentsKey))
                {
                    var sequences = availableContentsEvent.Sequences.Sequences;
                    var sequencer = new Sequencer(new Container(), sequences);
                    await sequencer.PlayAsync(cancellationToken);
                    userData.AvailableContents.RemoveNewContent(availableContentsEvent.NewContentsKey);
                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                }
            }

        }
    }
}
