
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class PooleabeObject : MonoBehaviour
    {
        public static PooleabeObject Create(PoolController pool, GameObject prefab)
        {
            GameObject go = Instantiate(prefab);
            PooleabeObject goPooler = go.GetComponent<PooleabeObject>();
            if (goPooler == null)
            {
                goPooler = go.AddComponent<PooleabeObject>();
            }

            goPooler.SetPool(pool);
            return goPooler;
        }

        private PoolController m_pool = null;

        public Vector3 Position => transform.position;

        private void Awake()
        {
            gameObject.SetActive(false);
        }
       
        public void SetPool(PoolController pool)
        {
            m_pool = pool;
        }
        public void Recycle()
        {

            m_pool.RecycleToPool(this);
        }

        internal virtual void OnPickFromPool(Vector3 position)
        {
            gameObject.transform.position = position;
            gameObject.SetActive(true);
        }
        internal virtual void OnStoredInPool()
        {
            gameObject.SetActive(false);
        }
        internal float GetDistance(Vector3 toPos)
        {
            return Vector3.Distance(Position, toPos);
        }

    }
}