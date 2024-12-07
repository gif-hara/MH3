using UnityEngine;

namespace MH3
{
    public static partial class Extensions
    {
        public static void DestroySafe(this Component component)
        {
            if (component != null)
            {
                Object.Destroy(component.gameObject);
            }
        }
    }
}
