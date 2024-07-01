using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace HayWay.Runtime.Components
{
    public static class LocalDataBase
    {
        private static PlayerDataBase m_data = null;
        private static ConfigDataBase m_config = null;
        public static PlayerDataBase PlayerData => m_data;

        public static bool IsLoaded => m_data != null && m_config != null;

        public static void Load()
        {
            Load_PlayerData();
            LoadConfig();
        }

        public static void Save()
        {
            SavePlayerData();
            SaveConfig();
            PlayerPrefs.Save();
        }

        private static void LoadConfig()
        {
            m_config = new ConfigDataBase();
        }
        private static void SaveConfig()
        {

        }
        private static void Load_PlayerData()
        {
            int version = PlayerPrefs.GetInt(PlayerDataBase.DATA_VERSION_KEY, 0);
            if (version != PlayerDataBase.DATA_VERSION)
            {
                PlayerPrefs.DeleteKey(PlayerDataBase.DATA_KEY);
                PlayerPrefs.SetInt(PlayerDataBase.DATA_VERSION_KEY, PlayerDataBase.DATA_VERSION);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey(PlayerDataBase.DATA_KEY))
            {
                m_data = new PlayerDataBase(0);
                return;
            }

            string json = PlayerPrefs.GetString(PlayerDataBase.DATA_KEY);
            m_data = JsonUtility.FromJson<PlayerDataBase>(json);
        }
        private static void SavePlayerData()
        {
            string json = JsonUtility.ToJson(m_data);
            PlayerPrefs.SetString(PlayerDataBase.DATA_KEY, json);
        }

    }
    [SerializeField]
    public class ConfigDataBase
    {
        public const int DATA_VERSION = 0;
        public const string DATA_KEY = "config";
        public const string DATA_VERSION_KEY = "config_version";

    }

    [SerializeField]
    public class PlayerDataBase
    {
        public const int DATA_VERSION = 0;
        public const string DATA_KEY = "player";
        public const string DATA_VERSION_KEY = "player_version";
        public static event Action<int> OnPlayerCoinsChanged;
        public static event Action<int> OnPlayerSelectedCharChanged;
        public static event Action<int> OnPlayerCharAdded;

        [SerializeField] private int m_coins;
        [SerializeField] private int m_character;
        [SerializeField] private List<int> m_characters;

        public PlayerDataBase(int coins)
        {
            m_coins = 0;
            m_character = -1;
            m_characters = new List<int>();
        }
        public int Coins => m_coins;
        public int Character => m_character;
        public List<int> Characters => m_characters.ToList();

        public void AddCoins(int coin)
        {
            m_coins += coin;
            OnPlayerCoinsChanged?.Invoke(coin);
        }
        public void RemoveCoins(int coin)
        {
            if (m_coins == 0) return;
            m_coins -= coin;
            if (m_coins < 0) { m_coins = 0; }
            OnPlayerCoinsChanged?.Invoke(coin);

        }
        public void SelectChar(int catalogueIndex)
        {
            if (m_character == catalogueIndex) return;
            m_character = catalogueIndex;
            OnPlayerSelectedCharChanged?.Invoke(catalogueIndex);
        }
        public void AddCharacter(int catalogueIndex)
        {
            if (!m_characters.Contains(catalogueIndex))
            {
                m_characters.Add(catalogueIndex);
                OnPlayerCharAdded?.Invoke(catalogueIndex);
            }
        }

    }
}