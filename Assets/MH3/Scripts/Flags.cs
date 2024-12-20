using System.Collections.Generic;

namespace MH3
{
    public sealed class Flags
    {
        private HashSet<string> flags = new();

        public bool Any => flags.Count > 0;

        public void Set(string flag)
        {
            flags.Add(flag);
        }

        public void Unset(string flag)
        {
            flags.Remove(flag);
        }
    }
}
