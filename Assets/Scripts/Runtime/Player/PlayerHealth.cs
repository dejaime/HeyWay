
using System;
using UnityEngine;
using UnityEngine.Events;
namespace HayWay.Runtime.Components
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        public static event Action<PlayerHealth> OnPlayerHealthChange;

        [SerializeField] private int m_startHealth = 3;
        [SerializeField] private int m_maxHealth = 3;
        [SerializeField] private int m_maxHealthMax = 10;
        [SerializeField] private UnityEvent<IDamageable> OnDamagedEvent;
        [SerializeField] private UnityEvent<IDamageable> OnDamagedRestoredEvent;

        private int currentHealth;

        private void Start()
        {
            currentHealth = m_startHealth;
            CheckHelath();
            OnPlayerHealthChange?.Invoke(this);
        }

        private void OnDisable()
        {
            OnPlayerHealthChange = null;
        }
        private void CheckHelath()
        {
            if (currentHealth > m_maxHealth)
            {
                currentHealth = m_maxHealth;
                return;
            }

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

        }

        //===================================================================
        // IDAMAGEABLE INTERFACE
        //================================================================
        public void TakeDamage(int value)
        {
            currentHealth -= value;
            CheckHelath();
            OnDamagedEvent?.Invoke(this);
            OnPlayerHealthChange?.Invoke(this);
        }
        public void RestoreHealth(int value)
        {
            currentHealth += value;
            CheckHelath();
            OnDamagedRestoredEvent?.Invoke(this);
            OnPlayerHealthChange?.Invoke(this);

        }
        public void RestoreFullHealth()
        {
            currentHealth = m_maxHealth;
            OnDamagedRestoredEvent?.Invoke(this);
            OnPlayerHealthChange?.Invoke(this);

        }
        public int GetHealth()
        {
            return currentHealth;
        }
        public int GetMaxHealth()
        {
            return m_maxHealth;
        }
        //===================================================================
        // PLAYER HEALTH
        //================================================================
        public bool IsFull()
        {
            return GetHealth() >= GetMaxHealth();
        }
        public void IncreaseMaxHealth()
        {
           
            if (m_maxHealth >= m_maxHealthMax) { return; }
            m_maxHealth += 1;
            RestoreHealth(1);
            
        }
    }
}