
using UnityEngine;
using UnityEngine.Events;


namespace HayWay.Runtime.Components
{
    public class SpawnHealth : SpawnableStagePartObject
    {
        [SerializeField] private int m_HealValue = 1;
        [SerializeField] private bool m_IncreaseMaxHealthIfFull = true;
        [SerializeField] private UnityEvent OnPickedEvent;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(PlayerTag))
            {
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth.IsFull() && m_IncreaseMaxHealthIfFull)
                {
                    playerHealth.IncreaseMaxHealth();
                }
                else
                {
                    playerHealth.RestoreHealth(m_HealValue);
                }
                OnPickedEvent?.Invoke();
                Recycle();
            }
        }
    }
}