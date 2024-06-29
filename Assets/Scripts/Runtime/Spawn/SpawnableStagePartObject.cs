
using UnityEngine;


namespace HayWay.Runtime.Components
{
    public abstract class SpawnableStagePartObject : PooleabeObject
    {

        [SerializeField] private StagePart m_StagePart;
        public StagePart StagePart => m_StagePart;
        public abstract Vector3 Size { get; }
        public abstract Vector3 SizeOffset { get; }
        public abstract SpawnableAreaType AreaType { get; }

        public override void Awake()
        {
            if (m_StagePart != null)
            {
                m_StagePart.AddSpaw(this);
            }
            base.Awake();

        }
        public void SetStage(StagePart stagePart)
        {
            m_StagePart = stagePart;
        }

        internal override void OnStoredInPool()
        {
            if (StagePart != null)
            {
                StagePart.RemoveSpawn(this);
            }
            base.OnStoredInPool();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            switch (AreaType)
            {
                case SpawnableAreaType.SPHERE:
                    Gizmos.DrawWireSphere(transform.position + this.SizeOffset, this.Size.magnitude);
                    break;
                case SpawnableAreaType.BOX:
                    Gizmos.DrawWireCube(transform.position + this.SizeOffset, this.Size);
                    break;
            }


        }
    }

    public enum SpawnableAreaType
    {
        BOX,
        SPHERE,
    }

}