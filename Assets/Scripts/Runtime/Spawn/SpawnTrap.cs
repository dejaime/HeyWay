
using UnityEngine;
using HayWay.Runtime.Economy;
using UnityEditor.SceneManagement;
using System.Data;
using System.Collections;

namespace HayWay.Runtime.Components
{
    public class SpawnTrap : SpawnableStagePartObject
    {

        [SerializeField] private int m_damage = 1;

        private void OnTriggerEnter(Collider collider)
        {

            if (!collider.CompareTag(PlayerTag)) { return; }

            IDamageable damageable = collider.GetComponent<IDamageable>();
            damageable.TakeDamage(m_damage);

        }



    }
}