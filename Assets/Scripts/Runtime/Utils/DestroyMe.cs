using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class DestroyMe : MonoBehaviour
    {
        [SerializeField] private float m_delay = 0f;

        private async void Awake()
        {
            if (m_delay <= 0)
            {
                Destroy(gameObject);
                return;
            }

            await Task.Delay((int)m_delay * 1000);


            Destroy(gameObject);

        }

       

    }
}