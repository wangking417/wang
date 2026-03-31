using System;

namespace FirepowerFullBlast.Core
{
    public static class GameEventBus
    {
        public static event Action<int, int> AmmoChanged;
        public static event Action<float, float> PlayerHealthChanged;
        public static event Action<int> ScoreChanged;
        public static event Action<string> WeaponChanged;
        public static event Action<string> StatusChanged;

        public static int CurrentScore { get; private set; }

        public static void RaiseAmmoChanged(int currentMagazine, int currentReserve)
        {
            AmmoChanged?.Invoke(currentMagazine, currentReserve);
        }

        public static void RaisePlayerHealthChanged(float currentHealth, float maxHealth)
        {
            PlayerHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public static void RaiseWeaponChanged(string weaponName)
        {
            WeaponChanged?.Invoke(weaponName);
        }

        public static void RaiseStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public static void AddScore(int amount)
        {
            CurrentScore += amount;
            ScoreChanged?.Invoke(CurrentScore);
        }

        public static void ResetScore()
        {
            CurrentScore = 0;
            ScoreChanged?.Invoke(CurrentScore);
        }
    }
}

