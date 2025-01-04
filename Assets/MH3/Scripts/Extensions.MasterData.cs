using UnityEngine;

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
    }
}
