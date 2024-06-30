using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class UIPlayerTravel : MonoBehaviour
    {
        [SerializeField] private PlayerController m_player;
        [SerializeField] private TMPro.TextMeshProUGUI m_TextTraveled;


        private void OnEnable()
        {
            StartCoroutine(IEUpdate());
        }

        IEnumerator IEUpdate()
        {
            while (true)
            {

                m_TextTraveled.text = m_player.CurentTraveledDistance.ToString("N0")+"m";
                yield return null;

            }
        }
    }
}