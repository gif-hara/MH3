using UnityEngine;

namespace MH3
{
    public static partial class Extensions
    {
        public static MasterData.SkillLevelValue GetFromLevel(this MasterData.SkillLevelValue.DictionaryList self, int level)
        {
            level = Mathf.Clamp(level, 0, self.List.Count - 1);
            return self.Get(level);
        }
    }
}
