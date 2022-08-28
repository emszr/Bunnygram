using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Nonogram
{
    public class CurrencyManager : Singleton <CurrencyManager>
    {
        public List<CurrencyItem> currencyItemsList;
        //public Dictionary<CurrencyItemType, CurrencyItem> currencyItemDictionary;

        public bool TryToDecreaseCurrencyAmount(CurrencyItemType currencyItemType, int amount, float duration = 0.5f)
        {
            CurrencyItem currencyItem = currencyItemsList.Find(x => x.currencyItemType == currencyItemType);

            if (currencyItem.targetAmount - amount >= 0)
            {
                currencyItem.ChangeAmount(currencyItem.targetAmount - amount, duration);
                return true;
            }
            return false;
        }
        public void IncreaseCurrencyAmount(CurrencyItemType currencyItemType, int amount, float duration = 0.5f)
        {
            CurrencyItem currencyItem = currencyItemsList.Find(x => x.currencyItemType == currencyItemType);
            currencyItem.ChangeAmount(currencyItem.targetAmount + amount, duration);
        }

        public CurrencyItem GetCurrencyItem(CurrencyItemType currencyItemType)
        {
            return currencyItemsList.Find(x => x.currencyItemType == currencyItemType);
        }

        public Action<int> GetCurrencyChangeAction(CurrencyItemType currencyItemType)
        {
            return currencyItemsList.Find(x => x.currencyItemType == currencyItemType).OnCurrencyAmountChange;
        }

        public int GetItemCount(CurrencyItemType currencyItemType)
        {

            return currencyItemsList.Find(x => x.currencyItemType == currencyItemType).targetAmount;
        }

        private void OnEnable()
        {
            foreach (CurrencyItem currencyItem in currencyItemsList)
            {
                currencyItem.Initialize();
            }
        }
    }
}