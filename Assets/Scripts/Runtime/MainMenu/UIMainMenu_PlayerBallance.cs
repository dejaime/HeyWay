using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class UIMainMenu_PlayerBallance : MonoBehaviour
    {
       [SerializeField] private TMPro.TextMeshProUGUI m_TextBallance;

        public void RefreshBallance()
        {
            m_TextBallance.text = LocalDataBase.PlayerData.Coins.ToString();
        }
    }
}