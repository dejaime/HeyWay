using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class UIPlayerCoins : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI m_TextCoins;

        private void OnEnable()
        {
            PlayerCoins.OnPlayerCoinsChanged += OnPlayerCoinsChanged;
        }

        private void OnDisable()
        {
            PlayerCoins.OnPlayerCoinsChanged -= OnPlayerCoinsChanged;
        }

        private void OnPlayerCoinsChanged(PlayerCoins playerCoins)
        {
            m_TextCoins.text = playerCoins.GetCoins().ToString();
        }
    }
}