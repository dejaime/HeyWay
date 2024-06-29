
using System;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public abstract class PooleabeObject : MonoBehaviour
    {
        public static event Action<PooleabeObject> OnPickFromPoolEvent;
        public static event Action<PooleabeObject> OnStoredInPoolEvent;

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

        public PoolController Pool => m_pool;
        public bool IsPolled => Pool != null;
        public bool IsActived => gameObject.activeSelf;
        public Vector3 Position => transform.position;

        public virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        public virtual void SetPool(PoolController pool)
        {
            m_pool = pool;
        }
        public virtual void Recycle()
        {

            m_pool.RecycleToPool(this);
        }

        /// <summary>
        /// Set the position and the parent object and after that active this object invoking the event <see cref="OnPickFromPoolEvent"/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        /// <param name="stayWorldPosition"></param>
        internal virtual void OnPickFromPool(Vector3 position, Transform parent = null, bool stayWorldPosition = true, params object[] args)
        {
            gameObject.transform.position = position;
            if (parent != null)
            {
                gameObject.transform.SetParent(parent, stayWorldPosition);
                if (stayWorldPosition == false)
                {
                    gameObject.transform.localPosition = position;
                }

            }
            gameObject.SetActive(true);
            OnPickFromPoolEvent?.Invoke(this);
           
        }

        /// <summary>
        /// Desactive this object setting any parent for null and invoking the event <see cref="OnStoredInPoolEvent"/>
        /// </summary>
        internal virtual void OnStoredInPool()
        {
            
            gameObject.transform.SetParent(null);
            gameObject.SetActive(false);
            OnStoredInPoolEvent?.Invoke(this);

        }
        internal float GetDistance(Vector3 toPos)
        {
            return Vector3.Distance(Position, toPos);
        }

    }
}