using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HayWay.Runtime.Components
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] UIMainMenu_PlayerBallance m_uiPlayerBallance;
        [SerializeField] UIMainMenu_Shop m_uiShop;
        [SerializeField] Button m_PlayButton;
        [SerializeField, Attributes.BuildScene] string m_gameScene;

        private void Awake()
        {
            m_PlayButton.interactable = false;
            if (!LocalDataBase.IsLoaded)
            {
                LocalDataBase.Load();
            }


            m_PlayButton.interactable = LocalDataBase.PlayerData.Character >= 0;
        }
        private void OnEnable()
        {
            PlayerDataBase.OnPlayerCoinsChanged += OnPlayerCoinsChanged;
            PlayerDataBase.OnPlayerSelectedCharChanged += OnPlayerSelectedCharChanged;
            PlayerDataBase.OnPlayerCharAdded += OnPlayerCharBought;
        }

        private void OnDisable()
        {
            PlayerDataBase.OnPlayerCoinsChanged -= OnPlayerCoinsChanged;
            PlayerDataBase.OnPlayerSelectedCharChanged -= OnPlayerSelectedCharChanged;
            PlayerDataBase.OnPlayerCharAdded -= OnPlayerCharBought;
        }

        private void Start()
        {
            m_uiPlayerBallance.RefreshBallance();
            m_uiShop.Refresh();
        }

        private void OnPlayerCoinsChanged(int obj)
        {
            m_uiPlayerBallance.RefreshBallance();
        }

        private void OnPlayerSelectedCharChanged(int character)
        {
            m_uiShop.Refresh();
            m_PlayButton.interactable = true;
        }
        private void OnPlayerCharBought(int obj)
        {
            m_uiShop.Refresh();
        }
        public void Play()
        {
            SceneManager.LoadScene(m_gameScene);
        }
    }
}