using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Nonogram
{
    public class ShopPanel : MonoBehaviour
    {
        public List<ShopItem> shopItemList;
        public GameObject rocketImageObject;
        public GameObject punchImageObject;
        public GameObject bombImageObject;
        public GameObject parentpanel;
        public GameObject diamondPanel;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI multiplierText;

        private ShopItem shopItem;

        public void SetReadyPanel(CurrencyItemType type)
        {
            if (type != CurrencyItemType.None)
            {
                ShopItem item = shopItemList.Find(x => x.type == type);
                Debug.Log(item);
                if (item != null)
                {
                    shopItem = item;
                    title.text = item.title;
                    description.text = item.descripton;
                    costText.text = item.itemCost.ToString();
                    multiplierText.text = "X"+item.amountToBeAdded.ToString();
                    OpenImageObject(type);
                    int amount = CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Diamond).amount- item.itemCost;
                    UpdateButton(shopItem,amount);
                }
             
            }
        }

        public void CloseShopPanel(UIManager manager)
        {
            shopItem = null;
            GameManager.instance.OnShopPanelChanged(CurrencyItemType.None);
            GameManager.instance.ChangeGameState(GameState.Playing);
            diamondPanel.transform.DOScale(Vector3.zero, 0.1f).From(Vector3.one).OnComplete(() => diamondPanel.gameObject.SetActive(false));
            gameObject.transform.DOScale(Vector3.zero, 0.1f).From(Vector3.one).OnComplete(() => gameObject.SetActive(false));
            manager.SetMaskState(manager.GetMainMask(), false);
        }


        public void EnableAnimation()
        {
            gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
            diamondPanel.SetActive(true);
            diamondPanel.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

        public void TryBuyingItem()
        {
            bool sold = CurrencyManager.instance.TryToDecreaseCurrencyAmount(CurrencyItemType.Diamond, shopItem.itemCost);
            int amount = CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Diamond).amount - shopItem.itemCost;
            if (sold)
            {
                CurrencyManager.instance.IncreaseCurrencyAmount(shopItem.type, shopItem.amountToBeAdded);
            }
            UpdateButton(shopItem,amount);
        }

        public void UpdateButton(ShopItem item,int checkAmount)
        {
            Debug.Log(checkAmount);
            if (checkAmount <=0)
            {
                button.image.color = Color.gray;
                button.interactable = false;
            }
            else
            {
                button.image.color = Color.white;
                button.interactable = true;
            }
        }
        public void OpenImageObject(CurrencyItemType type)
        {
            if (type == CurrencyItemType.Bomb)
            {
                bombImageObject.SetActive(true);
                rocketImageObject.SetActive(false);
                punchImageObject.SetActive(false);
            }
            else if (type == CurrencyItemType.HorizontalRocket)
            {
                bombImageObject.SetActive(false);
                rocketImageObject.SetActive(false);
                punchImageObject.SetActive(true);
            }
            else if (type==CurrencyItemType.VerticalRocket)
            {
                bombImageObject.SetActive(false);
                rocketImageObject.SetActive(true);
                punchImageObject.SetActive(false);
            }
           
        }

        public void OnEnable()
        {
            GameManager.instance.OnShopPanelChanged += SetReadyPanel;
            EnableAnimation();
        }
        private void OnDisable()
        {
            GameManager.instance.OnShopPanelChanged -= SetReadyPanel;
        }

    }
}

