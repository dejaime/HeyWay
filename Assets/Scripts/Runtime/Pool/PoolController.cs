using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    [DefaultExecutionOrder(-1000)]
    public class PoolController : MonoBehaviour
    {
        public GameObject prefab = null;
        public int startQnt = 10;
        public bool allowExpand = true;

        List<PooleabeBehaviour> activedPollers = new List<PooleabeBehaviour>();
        List<PooleabeBehaviour> unactivedPollers = new List<PooleabeBehaviour>();

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
        private void Expand()
        {
            PooleabeBehaviour pooler = PooleabeBehaviour.Create(this, prefab);
            unactivedPollers.Add(pooler);
        }

       
        /// <summary>
        /// Retorna um objeto da lista de pools que nao esta sendo usado no momento. Expand sera usado se nao ouver um objeto para pegar.
        /// Tenha em mente que este objeto nao esta pronto para ser usado, ele serve somente para buscar informacoes.
        /// Para spawn utilize <see cref="GetPool{T}(Vector3, Transform, bool, object[])"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSpyPool<T>() where T : PooleabeBehaviour
        {
            if (unactivedPollers.Count == 0 && allowExpand)
            {
                Expand();
            }

            PooleabeBehaviour go = unactivedPollers.FirstOrDefault();
            if (go == null) { return null; }
            return (T)go;

        }
        /// <summary>
        /// Retorna um objeto da lista de pools. Este objeto esta pronto para uso. 
        /// Se voce deseja receber um obejto apenas para espiar, utilize <see cref="GetSpyPool{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        /// <param name="stayWordposition"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T GetPool<T>(Vector3 position, Transform parent = null, bool stayWordposition = true, params object[] args) where T : PooleabeBehaviour
        {
            if (unactivedPollers.Count == 0 && allowExpand)
            {
                Expand();
            }

            PooleabeBehaviour go = unactivedPollers.FirstOrDefault();
            if (go == null) { return null; }
            unactivedPollers.Remove(go);
            activedPollers.Add(go);
            go.OnPickFromPool(position, parent, stayWordposition, args);

            return (T)go;
        }
        public void RecycleToPool(PooleabeBehaviour go)
        {
            if (activedPollers.Remove(go))
            {
                unactivedPollers.Add(go);
                go.OnStoredInPool();
            }
            else
            {

                Debug.LogError($"RecycleToPool {go?.name} fail bause it is not in the own list ");
            }
        }

        internal void SetToActiveds(PooleabeBehaviour pooleabeObject)
        {
            activedPollers.Add(pooleabeObject);
        }
    }
}