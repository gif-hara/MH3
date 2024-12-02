using UnityEngine;

namespace MH3
{
    [CreateAssetMenu(menuName = "MH3/GameRules")]
    public class GameRules : ScriptableObject
    {
        [SerializeField]
        private float earlyInputTime;
        public float EarlyInputTime => earlyInputTime;
    }
}
