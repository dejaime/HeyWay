using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HayWay.Runtime.Components
{
    public class PlayerCoins : MonoBehaviour
    {
        public static event Action<PlayerCoins> OnPlayerCoinsChanged;

        [SerializeField] private UnityEvent<PlayerCoins> OnCoinAddedEvent;
        [SerializeField] private UnityEvent<PlayerCoins> OnCoinRemovedEvent;

        int currentCoin = 0;

        private void Start()
        {
            OnPlayerCoinsChanged?.Invoke(this);
        }
        public int GetCoins()
        {
            return currentCoin;
        }
        public void AddCoin(int value)
        {
            currentCoin += value;
            OnCoinAddedEvent?.Invoke(this);
            OnPlayerCoinsChanged?.Invoke(this);
        }
        public void RemoveCoin(int value)
        {
            currentCoin -= value;
            OnCoinRemovedEvent?.Invoke(this);
            OnPlayerCoinsChanged?.Invoke(this);
        }

    }
}