using UnityEngine;

namespace MH3
{
    public class DamageData
    {
        public int Damage { get; set; }

        public int FlinchDamage { get; }

        public DamageData(int damage, int flinchDamage)
        {
            Damage = damage;
            FlinchDamage = flinchDamage;
        }
    }
}
