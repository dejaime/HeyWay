using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

namespace HayWay.Runtime.Components
{
    public class UIPlayerGameOver : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGameOver;
        [SerializeField] private TMPro.TextMeshProUGUI m_TextBallance;
        [SerializeField, Attributes.BuildScene] private string m_menuScene;

        PlayerController m_PlayerController;
        private void Awake()
        {
            m_canvasGameOver.gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            PlayerController.OnDead += OnDead;
            StageController.OnRestarted += OnRestarted;
        }

        private void OnDisable()
        {
            PlayerController.OnDead -= OnDead;
            StageController.OnRestarted -= OnRestarted;

        }

      

        private void Refresh()
        {

            m_canvasGameOver.gameObject.SetActive(m_PlayerController.IsDead);
            if (gameObject.activeSelf)
            {
                m_TextBallance.text = m_PlayerController.GetComponent<PlayerCoins>().GetCoins().ToString();
            }
        }
        private void OnDead(PlayerController controller)
        {
            m_PlayerController = controller;
            Refresh();
        }
        private void OnRestarted(StageController controller)
        {
            Refresh();
        }
        public void Options_Retry() 
        {
            if (!LocalDataBase.IsLoaded) { LocalDataBase.Load(); }

            LocalDataBase.PlayerData.AddCoins(m_PlayerController.GetComponent<PlayerCoins>().GetCoins());
            LocalDataBase.Save();
            m_PlayerController.Restart();
        }
        public void Options_Exit()
        {

            if (!LocalDataBase.IsLoaded) { LocalDataBase.Load(); }

            LocalDataBase.PlayerData.AddCoins(m_PlayerController.GetComponent<PlayerCoins>().GetCoins());
            LocalDataBase.Save();

            SceneManager.LoadScene(m_menuScene);
        }
    }
}