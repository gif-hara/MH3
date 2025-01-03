using UnityEngine;

namespace MH3
{
    public class DamageData
    {
        public int Damage { get; set; }

        public int FlinchDamage { get; }

        public Vector3 DamagePosition { get; }

        public bool IsCritical { get; }

        public DamageData(int damage, int flinchDamage, Vector3 damagePosition, bool isCritical)
        {
            Damage = damage;
            FlinchDamage = flinchDamage;
            DamagePosition = damagePosition;
            IsCritical = isCritical;
        }
    }
}
