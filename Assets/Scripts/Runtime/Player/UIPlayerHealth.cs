
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class UIPlayerHealth : MonoBehaviour
    {
        [SerializeField] private PoolController m_hearthPool;
        [SerializeField] private RectTransform m_hearthsContentPanel;

        private void OnEnable()
        {
            PlayerHealth.OnPlayerHealthChange += OnPlayerHealthChange;
        }
        private void OnDisable()
        {
            PlayerHealth.OnPlayerHealthChange -= OnPlayerHealthChange;

        }

        private void OnPlayerHealthChange(PlayerHealth playerHealth)
        {
            int curentHealth = playerHealth.GetHealth();

            Debug.Log(curentHealth);
            SimplePooleableObject[] hearts = m_hearthsContentPanel.GetComponentsInChildren<SimplePooleableObject>(false);
            if (hearts.Length < curentHealth)
            {
                for (int i = 0; i < (curentHealth - hearts.Length); i++)
                {
                    SimplePooleableObject simplePolled = m_hearthPool.GetPool<SimplePooleableObject>(Vector3.zero, m_hearthsContentPanel, false);
                }
                return;
            }

            if (hearts.Length > curentHealth)
            {
                for (int i = 0; i < (hearts.Length - curentHealth); i++)
                {
                    hearts[i].Recycle();
                }
            }
        }
    }
}