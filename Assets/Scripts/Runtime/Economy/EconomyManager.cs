using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace HayWay.Runtime.Economy
{
    public class EconomyManager : MonoBehaviour
    {
        private static EconomyManager m_instance = null;
        private static bool isDestroying = false;

        [SerializeField] private EconomyPlayer m_player = new EconomyPlayer();

        public EconomyPlayer Player => m_player;

        public static EconomyManager Instance
        {
            get
            {
                if (m_instance == null && isDestroying == false)
                {
                    GameObject go = new GameObject("Economy");
                    go.AddComponent<EconomyManager>();
                    DontDestroyOnLoad(go);
                }

                return m_instance;
            }
        }


        //This component is never disabling, just before destroying in exit application
        private void OnDisable()
        {
            isDestroying = true;
        }

    }
}