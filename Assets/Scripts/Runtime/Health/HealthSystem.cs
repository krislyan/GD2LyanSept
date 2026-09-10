using System;
using UnityEngine;
using UnityEngine.Events;

namespace Ludocore
{
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        [Header("Config")]
        [Tooltip("Data asset defining max health and damage cooldown")]
        [SerializeField] private HealthData data;

        [Header("Debug")]
        [ReadOnly, SerializeField] private float currentHealth;
        [ReadOnly, SerializeField] private bool isDead;

        private float _lastDamageTime = -Mathf.Infinity;

        public float CurrentHealth => currentHealth;
        public float MaxHealth     => data.MaxHealth;
        public float HealthRatio   => currentHealth / data.MaxHealth;
        public bool  IsDead        => isDead;

        public event Action<float> OnDamaged;
        public event Action<float> OnHealed;
        public event Action        OnDied;
        public event Action        OnHealthChanged;   // NEW: fires on any change, for UI

        [Header("Events")]
        [SerializeField] private UnityEvent<float> damagedEvent;
        [SerializeField] private UnityEvent<float> healedEvent;
        [SerializeField] private UnityEvent diedEvent;

        private void Awake()
        {
            currentHealth = data.MaxHealth;
            OnHealthChanged?.Invoke();              // NEW
        }

        public void TakeDamage(float amount)
        {
            if (isDead) return;
            if (amount <= 0f) return;
            if (Time.time - _lastDamageTime < data.DamageCooldown) return;

            _lastDamageTime = Time.time;
            currentHealth = Mathf.Max(0f, currentHealth - amount);

            OnDamaged?.Invoke(amount);
            damagedEvent?.Invoke(amount);
            OnHealthChanged?.Invoke();              // NEW

            if (currentHealth <= 0f) Die();
        }

        public void Heal(float amount)
        {
            if (isDead) return;
            if (amount <= 0f) return;

            currentHealth = Mathf.Min(data.MaxHealth, currentHealth + amount);

            OnHealed?.Invoke(amount);
            healedEvent?.Invoke(amount);
            OnHealthChanged?.Invoke();              // NEW
        }

        [ContextMenu("Take 10 Damage")]
        private void Debug_Take10() => TakeDamage(10f);
        [ContextMenu("Heal 10")]
        private void Debug_Heal10() => Heal(10f);
        [ContextMenu("Kill")]
        private void Debug_Kill() => TakeDamage(currentHealth);

        private void Die()
        {
            isDead = true;
            OnDied?.Invoke();
            diedEvent?.Invoke();
        }
    }
}