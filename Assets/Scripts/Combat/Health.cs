using System;
using FirepowerFullBlast.Core;
using UnityEngine;

namespace FirepowerFullBlast.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool isPlayer;
        [SerializeField] private int scoreValueOnDeath = 100;
        [SerializeField] private float destroyDelay = 3f;

        public event Action Died;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        private void Start()
        {
            if (isPlayer)
            {
                GameEventBus.RaisePlayerHealthChanged(CurrentHealth, maxHealth);
            }
        }

        public void TakeDamage(float amount, GameObject source)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);

            if (isPlayer)
            {
                GameEventBus.RaisePlayerHealthChanged(CurrentHealth, maxHealth);
                GameEventBus.RaiseStatusChanged(CurrentHealth > 0f ? "Under Fire" : "Player Down");
            }

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        public void RestoreFull()
        {
            IsDead = false;
            CurrentHealth = maxHealth;

            if (isPlayer)
            {
                GameEventBus.RaisePlayerHealthChanged(CurrentHealth, maxHealth);
            }
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            Died?.Invoke();

            if (!isPlayer)
            {
                GameEventBus.AddScore(scoreValueOnDeath);
            }

            if (destroyDelay > 0f)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}

