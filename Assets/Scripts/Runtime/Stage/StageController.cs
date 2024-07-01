using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HayWay.Runtime.Extensions;
using System;
using UnityEngine.SceneManagement;

namespace HayWay.Runtime.Components
{
    public class StageController : MonoBehaviour
    {
        public static event Action<StageController> OnLoopBack;
        public static event Action<StageController> OnRestarted;


        [SerializeField] private float m_lanePadding = 1;
        [SerializeField] private float m_maxTravelerDistance = 50;
        [SerializeField] private float m_recycleDistance = 10;
        [SerializeField] private List<StagePart> m_startActivedParts = new List<StagePart>();
        [SerializeField] private List<PoolController> m_parts = new List<PoolController>();
        [SerializeField] private List<SpawnStagePartBehaviour> m_spawners = new List<SpawnStagePartBehaviour>();

        public bool IsReady => isReady;

        List<StagePart> activedParts = new List<StagePart>();
        
        float lastRestoredTime = 0; //Prevent repetitive restoration of position
        bool isReady = false;
        bool isRestarting = false;

        private IEnumerator Start()
        {
            isReady = false;
            foreach (var part in m_startActivedParts)
            {
                activedParts.Add(part);
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            if (isRestarting)
            {
                OnRestarted?.Invoke(this);
                isRestarting = false;
            }

            isReady = true;
        }
        private void OnEnable()
        {
            PooleabeBehaviour.OnPickFromPoolEvent += OnPickFromPool;
        }
        private void OnDisable()
        {
            PooleabeBehaviour.OnPickFromPoolEvent -= OnPickFromPool;
            OnLoopBack = null;
        }
        private void OnDestroy()
        {
            OnLoopBack = null;
            OnRestarted = null;
        }
        private StagePart GetRandomPart(Vector3 position)
        {
            int rnd = UnityEngine.Random.Range(0, m_parts.Count);
            StagePart part = m_parts[rnd].GetPool<StagePart>(position, args: this);
            activedParts.Add(part);
            return activedParts.Last();
        }
        private StagePart GetLastActivedPart()
        {
            return (StagePart)activedParts.Last();
        }
        private StagePart GetActivednearPart(Vector3 position)
        {
            float distance = Mathf.Infinity;
            PooleabeBehaviour result = null;
            foreach (var part in activedParts)
            {
                float relativeDist = part.GetDistance(position);
                if (relativeDist > distance) { continue; }
                distance = relativeDist;
                result = part;

            }

            return (StagePart)result;
        }
        private float GetLastActivedPartPositionFoward()
        {
            StagePart lastpart = GetLastActivedPart();
            float z = lastpart.transform.position.z + lastpart.Size;
            return z;
        }
        private void EvalueActivedParts(PlayerController player)
        {
            //Recycling and creating new part
            List<StagePart> toRecycle = new List<StagePart>();

            //Get Recycling
            foreach (var part in activedParts)
            {
                bool isfoward = part.Position.z - player.Position.z > 0;
                if (isfoward) { continue; }
                if (part.GetDistance(player.Position) < m_recycleDistance) { continue; }
                toRecycle.Add(part);
            }

            //Recycling back and create new fowards
            foreach (var recyle in toRecycle)
            {
                if (activedParts.Remove(recyle))
                {
                    recyle.Recycle();
                    Vector3 spawnPart = Vector3.zero;
                    spawnPart.z = GetLastActivedPartPositionFoward();
                    GetRandomPart(spawnPart);
                }
            }
        }
        private void EvalueLoopBack(PlayerController player)
        {
            //Recover start position
            //***** ALGORITM**************************************************************************
            // -> I will to create a GameObject in the world in the same foward position of the player
            // -> So I will to put all actived parts and the player inner them
            // -> After that i will seting the GameObject position to (zero), so in the way
            // All actived parts and the player will toghetter for the restart position
            // -> In the end i will remove all parents and delete the gameObject.
            //
            // This is a simple but very efftivness way to restore my parts and player position
            // without complex math algoritm
            //***************************************************************************************

            if (Time.time < lastRestoredTime + 1) { return; }
            lastRestoredTime = Time.time;

            float travelerDistance = Vector3.Distance(player.Position, Vector3.zero);
            if (travelerDistance < m_maxTravelerDistance) { return; }

            GameObject group = new GameObject("GroupToRecover");
            group.transform.position = new Vector3(0, 0, player.Position.z);

            //Setting player and actived parts parent for the groupObject
            player.transform.SetParent(group.transform, true);
            foreach (var part in activedParts) { part.transform.SetParent(group.transform, true); }
            //Reset position of the group and all child objects
            group.transform.position = Vector3.zero;
            //Removing all paretns and removing the group object
            foreach (var part in activedParts) { part.transform.SetParent(null); }
            player.transform.SetParent(null);
            player.RefreshPosition();
            Destroy(group);
            OnLoopBack?.Invoke(this);
        }
        private void OnPickFromPool(PooleabeBehaviour po)
        {
            if (po is not StagePart) return;

            StartCoroutine(IESpawn(po));
        }
        IEnumerator IESpawn(PooleabeBehaviour po)
        {
            if (!po.IsActived) { yield break; }

            foreach (var spawner in m_spawners)
            {
                if (!po.IsActived) { yield break; }

                spawner.Execute((StagePart)po);
                yield return null;
            }
        }

        /// <summary>
        /// Get a X position from a given index padding. This inde can be positive or negative and ZERO is equal of the midle lane.
        /// <para>
        /// Note: This function has no limitation on the amount of index for the lane.
        /// </para>
        /// </summary>
        /// <param name="index">Negative index is left and positive index is right lane positions by index reference</param>
        /// <returns></returns>
        public float GetLane(int index)
        {
            return m_lanePadding * index;
        }
        public float GetRandomLane(int min, int max)
        {
            int index = UnityEngine.Random.Range(min, max + 1);
            return GetLane(index);
        }
        public void UpdateStage(PlayerController player)
        {
            EvalueActivedParts(player);
            EvalueLoopBack(player);
        }
        public void Restart()
        {

            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);

                /*
            isReady = false;
            isRestarting = true;
            foreach (var part in activedParts)
            {
                part.Recycle();
            }

            activedParts.Clear();
            StartCoroutine(Start());*/
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < 3; i++)
            {
                int index = i - 1;

                Vector3 startPoint = Vector3.right * m_lanePadding * index;
                Vector3 endPoint = startPoint + Vector3.forward * m_maxTravelerDistance;

                Gizmos.DrawLine(startPoint, endPoint);
            }

        }

    }
}