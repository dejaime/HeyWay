using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class PoolController : MonoBehaviour
    {
        public GameObject prefab = null;
        public int startQnt = 10;
        public bool allowExpand = true;

        List<PooleabeObject> activedPollers = new List<PooleabeObject>();
        List<PooleabeObject> unactivedPollers = new List<PooleabeObject>();

        private void Awake()
        {
            CreatInitialPool();
        }

        private void CreatInitialPool()
        {
            for (int i = 0; i < startQnt; i++)
            {
                Expand();
            }
        }
        void Expand()
        {
            PooleabeObject pooler = PooleabeObject.Create(this, prefab);
            unactivedPollers.Add(pooler);
        }
        public PooleabeObject GetPool(Vector3 position)
        {
            if (unactivedPollers.Count == 0 && allowExpand)
            {
                Expand();
            }

            PooleabeObject go = unactivedPollers.FirstOrDefault();
            if (go == null) { return null; }
            unactivedPollers.Remove(go);
            activedPollers.Add(go);
            go.OnPickFromPool(position);
            return go;

        }
        public void RecycleToPool(PooleabeObject go)
        {
            activedPollers.Remove(go);
            unactivedPollers.Add(go);
            gameObject.SetActive(false);
            go.OnStoredInPool();
        }


    }
}