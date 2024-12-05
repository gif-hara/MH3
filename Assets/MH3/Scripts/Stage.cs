using UnityEngine;

namespace MH3
{
    public class Stage : MonoBehaviour
    {
        [SerializeField]
        private Transform playerSpawnPoint;
        public Transform PlayerSpawnPoint => playerSpawnPoint;
        
        [SerializeField]
        private Transform enemySpawnPoint;
        public Transform EnemySpawnPoint => enemySpawnPoint;
    }
}
