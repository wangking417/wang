using UnityEngine;

namespace FirepowerFullBlast.Combat
{
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Firepower Full Blast/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string weaponName = "Assault Rifle";

        [Header("Damage")]
        public float damage = 20f;
        public float range = 120f;
        public float fireRate = 10f;

        [Header("Ammo")]
        public int magazineSize = 30;
        public int reserveAmmo = 90;
        public float reloadTime = 1.8f;

        [Header("Handling")]
        public bool automatic = true;
        public float hipSpread = 0.015f;
        public LayerMask hitMask = ~0;
    }
}
