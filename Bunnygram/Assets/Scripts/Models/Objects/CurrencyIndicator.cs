using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Nonogram
{

    public class CurrencyIndicator : MonoBehaviour
    {
        public CurrencyItemType currencyItemType;
        public TextMeshProUGUI amountText;
        public bool dynamicUpdate;
        
        private void Start()
        {
            if (currencyItemType == CurrencyItemType.Diamond)
            {
                int totalDiamond = CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Diamond).amount;
                int tempDiamond = LevelManager.instance.GetCollectedItems(CurrencyItemType.Diamond);

                if (tempDiamond > 0&&totalDiamond>=tempDiamond)
                {
                    amountText.text = (totalDiamond - tempDiamond).ToString();
                }
                else
                {
                    amountText.text = totalDiamond.ToString();
                }
            }
            else if (currencyItemType == CurrencyItemType.Star)
            {
                int totalDiamond = CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Star).amount;
                int tempDiamond = LevelManager.instance.GetCollectedItems(CurrencyItemType.Star);

                if (tempDiamond > 0 && totalDiamond >= tempDiamond)
                {
                    amountText.text = (totalDiamond - tempDiamond).ToString();
                }
                else
                {
                    amountText.text = totalDiamond.ToString();
                }
            }
        }

        public void ChangeAmount(int amount)
        {
            amountText.text = amount.ToString();
        }

        public void SetCurrencyText(int startAmount,int endAmount,float duration)
        {
            int end = endAmount;
            DOTween.To(() => startAmount, x => startAmount = x, endAmount, duration).OnUpdate(
                () => ChangeAmount(startAmount)
                );
        }

        private void OnEnable()
        {
    
            if (dynamicUpdate)
            {
                ChangeAmount(CurrencyManager.instance.GetItemCount(currencyItemType));
                CurrencyManager.instance.GetCurrencyItem(currencyItemType).OnCurrencyAmountChange += ChangeAmount;
            }
        }

        private void OnDisable()
        {
            if (dynamicUpdate)
            {
                CurrencyManager.instance.GetCurrencyItem(currencyItemType).OnCurrencyAmountChange -= ChangeAmount;
            }
        } 
    }
}