using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Nonogram
{
    public class PowerupButton : MonoBehaviour
    {
        [SerializeField] private Image redImage;
        [SerializeField] private Image greenImage;
        [SerializeField] private CurrencyItemType currencyItemType;
        private bool isZero = false;
        public bool isSelected = false;
      
        public void OnTouch()
        {
            if (!isSelected)
            {
                if(CurrencyManager.instance.GetCurrencyItem(currencyItemType).amount <= 0)
                {
                    GameManager.instance.ChangeGameState(GameState.Shop);
                    GameManager.instance.OnShopPanelChanged(currencyItemType);
                    return;
                }
                GameManager.instance.ChangeGameState(GameState.Playing | GameState.PowerUp);
                GameManager.instance.OnPowerButtonPushed(currencyItemType);
                isSelected = true;
                transform.DOKill();
                transform.DOScale(Vector3.one * 1.2f, 0.3f).From(Vector3.one);
            }

            else
            {
                GameManager.instance.OnPowerButtonPushed(currencyItemType);
                GameManager.instance.ChangeGameState(GameState.Playing);
                isSelected = false;
                transform.DOKill();
                transform.DOScale(Vector3.one, 0.2f);
            }
        }

        private void ModifyBackground(int amount)
        {
            if(isZero && amount > 0)
            {
                isZero = false;
                greenImage.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => redImage.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero));
            }

            else if(!isZero && amount <= 0)
            {
                isZero = true;
                redImage.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => greenImage.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero));
            }
        }

        public void Initialize(int amount)
        {
            if(amount <= 0)
            {
                isZero = true;
                redImage.transform.localScale = Vector3.zero;
                greenImage.transform.localScale = Vector3.one;
            }
        }

        private void Close(CurrencyItemType currencyItemType)
        {
            if (isSelected)
            {
                isSelected = false;
                transform.DOScale(Vector3.one, 0.2f);
            }     
        }

        public void ExitPowerUp(Cell cell)
        {
            if (isSelected)
            {
                isSelected = false;
                transform.DOScale(Vector3.one, 0.2f);
            }
        }

        private void OnEnable()
        {
            Initialize(CurrencyManager.instance.GetCurrencyItem(currencyItemType).amount);
            CurrencyManager.instance.GetCurrencyItem(currencyItemType).OnCurrencyAmountChange += ModifyBackground;
            GameManager.instance.OnPowerButtonPushed += Close;
            GameManager.instance.OnUsePowerUp += ExitPowerUp;
        }

        private void OnDisable()
        {
            CurrencyManager.instance.GetCurrencyItem(currencyItemType).OnCurrencyAmountChange -= ModifyBackground;
            GameManager.instance.OnPowerButtonPushed -= Close;
            GameManager.instance.OnUsePowerUp -= ExitPowerUp;
        }
    }
}
