#if UNITY_EDITOR
using System.IO;
using FirepowerFullBlast.Combat;
using UnityEditor;
using UnityEngine;

namespace FirepowerFullBlast.EditorTools
{
    public static class SampleContentGenerator
    {
        [MenuItem("火力全开/Generate Sample Assets")]
        public static void GenerateSampleAssets()
        {
            EnsureDirectory("Assets/Data");
            EnsureDirectory("Assets/Data/Weapons");

            CreateWeaponAsset(
                "AssaultRifle",
                "Assets/Data/Weapons/AssaultRifle.asset",
                24f,
                140f,
                10f,
                30,
                120,
                1.8f,
                true);

            CreateWeaponAsset(
                "HeavyPistol",
                "Assets/Data/Weapons/HeavyPistol.asset",
                34f,
                90f,
                4f,
                12,
                48,
                1.3f,
                false);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("火力全开", "示例武器资源已生成到 Assets/Data/Weapons。", "确定");
        }

        private static void CreateWeaponAsset(
            string weaponName,
            string assetPath,
            float damage,
            float range,
            float fireRate,
            int magazineSize,
            int reserveAmmo,
            float reloadTime,
            bool automatic)
        {
            WeaponDefinition asset = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<WeaponDefinition>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            asset.weaponName = weaponName;
            asset.damage = damage;
            asset.range = range;
            asset.fireRate = fireRate;
            asset.magazineSize = magazineSize;
            asset.reserveAmmo = reserveAmmo;
            asset.reloadTime = reloadTime;
            asset.automatic = automatic;
            EditorUtility.SetDirty(asset);
        }

        private static void EnsureDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            string folderName = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureDirectory(parent);
            }

            if (!string.IsNullOrEmpty(parent))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
#endif
