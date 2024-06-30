
using System.Collections;
using UnityEngine;


namespace HayWay.Runtime.Components
{
    [RequireComponent(typeof(Collider))]
    public abstract class SpawnableStagePartObject : PooleabeBehaviour
    {

        [SerializeField] private StagePart m_StagePart;
        [SerializeField] private Vector3 m_size = Vector3.one;
        [SerializeField] private Vector3 m_sizeOffset = Vector3.zero;
        [SerializeField] private SpawnableAreaType m_areaType = SpawnableAreaType.BOX;
        [SerializeField, Attributes.Tag] private string m_playerTag = "";
        [SerializeField, Attributes.Tag] private string m_obstacleTag = "";
        [SerializeField] private LayerMask m_checkLayerMask;
        [SerializeField] private bool m_destroyObstacles = false;

        private Collider m_Collider;

        public StagePart StagePart => m_StagePart;
        public string PlayerTag => m_playerTag;
        public string ObstacleTag => m_obstacleTag;
        public bool DestroyObstacles => m_destroyObstacles;

        public Vector3 Size => m_size;
        public Vector3 SizeOffset => m_sizeOffset;
        public SpawnableAreaType AreaType => m_areaType;

        public override void Awake()
        {
            if (m_StagePart != null)
            {
                m_StagePart.AddSpaw(this);
            }
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;

            base.Awake();

        }
        public virtual void OnEnable()
        {
            if (!this.IsPolled) { return; }
            if (!DestroyObstacles) { return; }
            StartCoroutine(IECheckCollide());
        }

        IEnumerator IECheckCollide()
        {
            //Wait for the next physics frame, because Raycasts work in FixedUpdate
            yield return new WaitForFixedUpdate();
            Collider[] colliders = null;

            if (AreaType == SpawnableAreaType.BOX)
            {
                colliders = Physics.OverlapBox(transform.position + SizeOffset, Size * 0.5f, transform.rotation, m_checkLayerMask, QueryTriggerInteraction.Collide);
            }
            else
            {
                colliders = Physics.OverlapSphere(transform.position + SizeOffset, Size.magnitude, m_checkLayerMask, QueryTriggerInteraction.Collide);
            }

            foreach (var collider in colliders)
            {
                if (!collider.CompareTag(m_obstacleTag)) { continue; }
                if (collider.gameObject == gameObject) { continue; }
                if (collider.TryGetComponent<SpawnableStagePartObject>(out var p_obj))
                {
                    p_obj.Recycle();
                }
            }
        }

        internal void SetStage(StagePart stagePart)
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


        private void ExecuteSpherePhysics() { }
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