
using UnityEngine;


namespace HayWay.Runtime.Components
{
    [RequireComponent(typeof(Collider))]
    public class SpawnCoin : SpawnableStagePartObject
    {
        [SerializeField] private Vector3 m_size = Vector3.one;
        [SerializeField] private Vector3 m_sizeOffset = Vector3.zero;
        [SerializeField] private SpawnableAreaType m_areaType = SpawnableAreaType.SPHERE;
        [SerializeField, Attributes.Tag] private string m_playerTag = "";
        [SerializeField, Attributes.Tag] private string m_obstacleTag = "";

        private Collider m_Collider;

        public override Vector3 Size => m_size;
        public override Vector3 SizeOffset => m_sizeOffset;

        public override SpawnableAreaType AreaType => m_areaType;

        public override void Awake()
        {
            base.Awake();
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;
        }

        private void OnEnable()
        {

        }
        private void OnTriggerEnter(Collider collider)
        {

            if (collider.CompareTag(m_playerTag))
            {

                Recycle();
            }

        }

       

    }
}