
using UnityEngine;
using HayWay.Runtime.Economy;
using UnityEditor.SceneManagement;
using System.Data;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

namespace HayWay.Runtime.Components
{
    public class SpawnEnemy : SpawnableStagePartObject
    {

        [SerializeField] private int m_damage = 1;
        [SerializeField] private float m_speed = 1;

        public override void OnEnable()
        {
            base.OnEnable();
            if (this == null) { return; }
            if (IsRecycling) { return; }
            StartCoroutine(IEUpdate());
        }

        IEnumerator IEUpdate()
        {
            if (IsRecycling) { yield break; }

            bool runing = true;
            int currentLane = -1;
            WaitForSeconds waitsecconds = new WaitForSeconds(1);

            while (runing)
            {
                
                var step = m_speed * Time.deltaTime; 
                Vector3 destination = Vector3.zero;
                destination.x = currentLane;
                destination.y = 0;
                destination.z = transform.position.z;

                transform.position = Vector3.MoveTowards(transform.position, destination, step);

                // Check if the position of the cube and sphere are approximately equal.
                if (Vector3.Distance(transform.position,destination) < 0.001f)
                {
                    // Swap the position of the cylinder.
                    currentLane = Random.Range((int)-1, (int)2);
                    yield return waitsecconds;
                }

                yield return null;
            }


        }

        private void OnTriggerEnter(Collider collider)
        {

            if (!collider.CompareTag(PlayerTag)) { return; }

            IDamageable damageable = collider.GetComponent<IDamageable>();
            damageable.TakeDamage(m_damage);

        }



    }
}