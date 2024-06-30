
using UnityEngine;
using UnityEngine.Events;


namespace HayWay.Runtime.Components
{
    public class SpawnCoin : SpawnableStagePartObject
    {
        [SerializeField] private int m_coinQnt = 1;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(PlayerTag))
            {
                PlayerCoins playerCoins = collider.GetComponent<PlayerCoins>();
                playerCoins.AddCoin(m_coinQnt);
                Recycle();
            }
        }
    }
}