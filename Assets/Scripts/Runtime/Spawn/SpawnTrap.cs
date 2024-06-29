
using UnityEngine;
using HayWay.Runtime.Economy;
using UnityEditor.SceneManagement;
using System.Data;
using System.Collections;

namespace HayWay.Runtime.Components
{
    [RequireComponent(typeof(Collider))]
    public class SpawnTrap : SpawnableStagePartObject
    {
        [SerializeField] private Vector3 m_size = Vector3.one;
        [SerializeField] private Vector3 m_sizeOffset = Vector3.zero;
        [SerializeField] private SpawnableAreaType m_areaType = SpawnableAreaType.BOX;
        [SerializeField, Attributes.Tag] private string m_playerTag = "";
        [SerializeField, Attributes.Tag] private string m_obstacleTag = "";
        [SerializeField] private bool m_destroyOthers = false;

        private Collider m_Collider;

        public override Vector3 Size => m_size;
        public override SpawnableAreaType AreaType => m_areaType;
        public override Vector3 SizeOffset => m_sizeOffset;

        public override void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;
            base.Awake();
        }

        private void OnEnable()
        {
            if (!this.IsPolled) { return; }
            if (!m_destroyOthers) return;
            StartCoroutine(IECheckCollide());
        }

        IEnumerator IECheckCollide()
        {
            //Wait for the next physics frame, because Raycasts work in FixedUpdate
            yield return new WaitForFixedUpdate();
            switch (m_areaType)
            {
                case SpawnableAreaType.BOX:
                    ExecuteBoxPhysics();
                    break;
            }
        }
        private void ExecuteBoxPhysics()
        {

            Collider[] colliders = Physics.OverlapBox(transform.position + m_sizeOffset, Size * 0.5f, transform.rotation);

            foreach (var collider in colliders)
            {

                if (!collider.CompareTag(m_obstacleTag)) { continue; }
                if (collider == m_Collider) { continue; }
                if (collider.TryGetComponent<SpawnableStagePartObject>(out var p_obj))
                {
                    p_obj.Recycle();
                }
            }

        }
        private void ExecuteSpherePhysics() { }

        private void OnTriggerEnter(Collider collider)
        {

            if (collider.CompareTag(m_playerTag))
            {
                return;
            }


        }



    }
}