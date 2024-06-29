using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Economy
{
    [System.Serializable]
    public class EconomyPlayer
    {
        [SerializeField] private int m_coins;
        public int Coins => m_coins;

        public void AddCoins(int coins)
        {
            m_coins += coins;
            EconomyManager.Instance.Player.AddCoins(coins);
        }
        
    }
}