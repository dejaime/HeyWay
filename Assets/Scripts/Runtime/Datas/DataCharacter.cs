using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    [CreateAssetMenu(menuName = "HayWay/Character")]
    public class DataCharacter : ScriptableObject
    {
        [SerializeField] string m_name = Strings.UNDEFINED;
        [SerializeField] int m_price = 750;
        [SerializeField] int m_startHearts = 3;
        [SerializeField] int m_maxHearts = 10;
        [SerializeField] Sprite m_portrate;
        [SerializeField] GameObject model;

        public string Name => m_name;
        public int Price => m_price;
        public int StartHearts => m_startHearts;
        public int MaxHearts => m_maxHearts;
        public Sprite ImagePortrate => m_portrate;
        public GameObject Model => model;
    }
}