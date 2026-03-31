using UnityEngine;

namespace FirepowerFullBlast.Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount, GameObject source);
    }
}

