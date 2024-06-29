using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public abstract class SpawnStagePartBehaviour : MonoBehaviour
    {
        [SerializeField] private StageController m_Stage;
        [SerializeField] private PoolController m_Pool;
        [SerializeField] private SpawnStagePartTye m_SpawnType;
        //[SerializeField] private int m_priority = 0;

        [SerializeField, Range((int)0, (int)100)] private int m_ProbabilityLeftLane = 0;
        [SerializeField, Range((int)0, (int)100)] private int m_ProbabilityMidleLane = 100;
        [SerializeField, Range((int)0, (int)100)] private int m_ProbabilityRightLane = 0;

       // public int Priority => m_priority;

        public virtual void Execute(StagePart part)
        {
            if (!enabled) { return; }
            if (!this.isActiveAndEnabled) { return; }
            if(gameObject.activeSelf==false) { return; }

            switch (m_SpawnType)
            {
                case SpawnStagePartTye.LANE_RANDOM:
                    SpawnRandomLane(part);
                    break;
                case SpawnStagePartTye.LANE_PROBALISTIC:
                    SpawnProbalisticLane(part);
                    break;
                case SpawnStagePartTye.LEFT_LANE_PROBALISTIC:
                    SpawnLane_Probalistic_Left(part);
                    break;
                case SpawnStagePartTye.RIGHT_LANE_PROBALICTIC:
                    SpawnLane_Probalistic_Right(part);
                    break;
                case SpawnStagePartTye.MIDLE_LANE_PROBALISTIC:
                    SpawnLane_Probalistic_Midle(part);
                    break;
                case SpawnStagePartTye.LEFT_LANE:
                    SpawnLane(part, -1, 100);
                    break;
                case SpawnStagePartTye.RIGHT_LANE:
                    SpawnLane(part, 1, 100);
                    break;
                case SpawnStagePartTye.MIDLE_LANE:
                    SpawnLane(part, 0, 100);
                    break;

            }
        }
        void SpawnRandomLane(StagePart part)
        {
            StartCoroutine(IESpawnRandomLane(part));
        }
        IEnumerator IESpawnRandomLane(StagePart part)
        {
            if (!part.IsActived) { yield break; }
            int partSize = (int)part.Size;
            for (int i = 0; i < partSize; i++)
            {
                float lanePosX = part.Stage.GetRandomLane(-1, 1);
                float lanePosZ = -(partSize * 0.5f) + i;
                Vector3 spawnPos = new Vector3(lanePosX, 0, lanePosZ);
                var obj = m_Pool.GetPool<SpawnableStagePartObject>(spawnPos, part.transform, false);
                part.AddSpaw(obj);
                yield return null;
            }
        }
        void SpawnProbalisticLane(StagePart part)
        {
            SpawnLane_Probalistic_Left(part);
            SpawnLane_Probalistic_Midle(part);
            SpawnLane_Probalistic_Right(part);
        }
        void SpawnLane(StagePart part, int lane, int probability)
        {
            if(probability == 0) return;
            StartCoroutine(IESpawnLane(part,lane,probability));
        }
        IEnumerator IESpawnLane(StagePart part, int lane, int probability)
        {
            if (!part.IsActived) { yield break; }

            int partSize = (int)part.Size;
            float lanePosX = part.Stage.GetLane(lane);

            for (int i = 0; i < partSize; i++)
            {
                if (!part.IsActived) { yield break; }
                int percentage = UnityEngine.Random.Range(0, 101);
                if (percentage <= probability)
                {
                    float lanePosZ = -(partSize * 0.5f) + i;
                    Vector3 spawnPos = new Vector3(lanePosX, 0, lanePosZ);
                    var obj = m_Pool.GetPool<SpawnableStagePartObject>(spawnPos, part.transform, false);
                    part.AddSpaw(obj);
                }
                yield return null;
            }
        }
        void SpawnLane_Probalistic_Left(StagePart part)
        {
            SpawnLane(part, -1, m_ProbabilityLeftLane);
        }
        void SpawnLane_Probalistic_Midle(StagePart part)
        {
            SpawnLane(part, 0, m_ProbabilityMidleLane);
        }
        void SpawnLane_Probalistic_Right(StagePart part)
        {
            SpawnLane(part, 1, m_ProbabilityRightLane);
        }

    }

    public enum SpawnStagePartTye
    {
        LANE_RANDOM,
        LANE_PROBALISTIC,
        LEFT_LANE_PROBALISTIC,
        RIGHT_LANE_PROBALICTIC,
        MIDLE_LANE_PROBALISTIC,
        LEFT_LANE,
        RIGHT_LANE,
        MIDLE_LANE,

    }
}