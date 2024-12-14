using UnityEngine;

namespace MH3.ProjectileControllers
{
    public class Projectile : MonoBehaviour
    {
        public Projectile Spawn(Vector3 position, Quaternion rotation)
        {
            var projectile = Instantiate(this, position, rotation);
            return projectile;
        }
    }
}
