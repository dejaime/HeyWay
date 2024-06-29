using System;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class StagePart : PooleabeObject
    {
        [SerializeField] private float m_size = 6;

        public float Size => m_size;
        public StageController Stage => m_stage;

        StageController m_stage;
        List<SpawnableStagePartObject> m_spawns = new List<SpawnableStagePartObject>();

        internal override void OnStoredInPool()
        {
            RecycleMaySpawns();
            base.OnStoredInPool();
        }
        internal override void OnPickFromPool(Vector3 position, Transform parent = null, bool stayWorldPosition = true, params object[] args)
        {
            m_stage = (StageController)args[0];
            base.OnPickFromPool(position, parent, stayWorldPosition,args);
        }

        /// <summary>
        /// Add this coin in this stage part setting the stagePart of the coin.
        /// </summary>
        /// <param name="coin"></param>
        internal void AddSpaw(SpawnableStagePartObject spawnObj)
        {
            m_spawns.Add(spawnObj);
            spawnObj.SetStage(this);
        }

        /// <summary>
        /// Remove this coin of this stage part Seting the coin stage to null
        /// </summary>
        /// <param name="coin"></param>
        internal void RemoveSpawn(SpawnableStagePartObject spawnObj)
        {
            m_spawns.Remove(spawnObj);
            spawnObj.SetStage(null);
        }

        private void RecycleMaySpawns()
        {
            //Recycle my Coins
            foreach (var coin in m_spawns)
            {
                coin.SetStage(null);
                coin.Recycle();
            }
            m_spawns.Clear();
        }
      
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            float cellSize = 1;
            float width = Size;
            float height = Size;

            float gridWidth = width * cellSize;
            float gridHeight = height * cellSize;

            Vector3 origin = transform.position - new Vector3(gridWidth / 2, 0, gridHeight / 2);

            for (int x = 0; x <= width; x++)
            {
                Gizmos.DrawLine(
                    origin + new Vector3(x * cellSize, 0, 0),
                    origin + new Vector3(x * cellSize, 0, gridHeight)
                );
            }

            for (int y = 0; y <= height; y++)
            {
                Gizmos.DrawLine(
                    origin + new Vector3(0, 0, y * cellSize),
                    origin + new Vector3(gridWidth, 0, y * cellSize)
                );
            }
        }


    }

}