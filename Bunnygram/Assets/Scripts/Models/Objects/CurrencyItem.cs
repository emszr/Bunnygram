using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace Nonogram
{
    [Serializable]
    public class CurrencyItem
    {
        [SerializeField] int defaultAmount;
        public int amount;
        public int targetAmount;
        public string playerPrefKey;
        public CurrencyItemType currencyItemType;
        public Action<int> OnCurrencyAmountChange;

        public void Initialize()
        {
            if (PlayerPrefs.HasKey(playerPrefKey))
            {
                amount = targetAmount = PlayerPrefs.GetInt(playerPrefKey);
            }
            else
            {
                amount = targetAmount = defaultAmount;
            }
        }

        public void ChangeAmount(int amount, float duration)
        {
            targetAmount = amount;
            PlayerPrefs.SetInt(playerPrefKey, targetAmount);
            DOTween.To(() => this.amount, x => this.amount = x, targetAmount, duration).OnUpdate(() =>
            OnCurrencyAmountChange?.Invoke(this.amount));
        }
    }
}