using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace HayWay.Runtime.Components
{
    public class UIMainMenu_Shop : MonoBehaviour
    {
        [SerializeField] private SwipeDetection m_swipeDectection;
        [SerializeField] private DataCharactersCatalogue m_Catalogue;
        [SerializeField] private TMPro.TextMeshProUGUI m_CatalogueCharName;
        [SerializeField] private TMPro.TextMeshProUGUI m_CatalogueCharPrice;
        [SerializeField] private Image m_CatalogueCharImage;
        [SerializeField] private RectTransform m_SelectedImage;
        [SerializeField] private Button m_ButtonBuy;
        [SerializeField] private Button m_Select;

        int currentShopChar = 0;

        private void Awake()
        {
            m_SelectedImage.gameObject.SetActive(false);
            m_ButtonBuy.gameObject.SetActive(false);
            m_Select.gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            m_swipeDectection.swipePerformed += OnSwipePerformed;
        }
        private void OnDisable()
        {
            m_swipeDectection.swipePerformed -= OnSwipePerformed;

        }

        public void Refresh()
        {
            PlayerDataBase playerData = LocalDataBase.PlayerData;

            DataCharacter character = m_Catalogue.GetCharacter(currentShopChar);

            m_CatalogueCharName.text = character.Name;
            m_CatalogueCharPrice.text = character.Price > 0 ? character.Price.ToString() : "Free";
            m_CatalogueCharImage.overrideSprite = character.ImagePortrate;

            bool isBought = playerData.Characters.Contains(currentShopChar);
            bool isSelected = playerData.Character == currentShopChar;

            m_ButtonBuy.interactable = true;

            if (playerData != null)
            {
                if (isBought)
                {
                    m_CatalogueCharPrice.text = "Acquired";
                }
                m_SelectedImage.gameObject.SetActive(isSelected && isBought);
                m_ButtonBuy.gameObject.SetActive(!isBought);
                m_Select.gameObject.SetActive(isBought);
                m_Select.interactable = !isSelected;
            }
        }
        public void ShowNextCharacter()
        {
            currentShopChar++;
            if (currentShopChar >= m_Catalogue.GetCharactersCount())
            {
                currentShopChar = 0;
            }
            Refresh();
        }
        public void ShowPrevCharacter()
        {
            currentShopChar--;
            if (currentShopChar < 0)
            {
                currentShopChar = m_Catalogue.GetCharactersCount() - 1;
            }
            Refresh();
        }
        public void Buy()
        {
            DataCharacter character = m_Catalogue.GetCharacter(currentShopChar);
            PlayerDataBase playerData = LocalDataBase.PlayerData;
            if (playerData.Coins < character.Price) { return; }

            m_ButtonBuy.interactable = false;
            LocalDataBase.PlayerData.RemoveCoins(character.Price);
            LocalDataBase.PlayerData.AddCharacter(currentShopChar);
            LocalDataBase.Save();

        }
        public void Select()
        {
            PlayerDataBase playerData = LocalDataBase.PlayerData;
            bool isBought = playerData.Characters.Contains(currentShopChar);
            bool isSelected = playerData.Character == currentShopChar;

            if (isBought && !isSelected)
            {
                playerData.SelectChar(currentShopChar);
            }

            LocalDataBase.Save();
        }

        private void OnSwipePerformed(Vector2 direction)
        {
            if (direction.x > 0)
            {
                ShowNextCharacter();
            }
            else if (direction.x < 0)
            {
                ShowPrevCharacter();
            }
        }
    }
}