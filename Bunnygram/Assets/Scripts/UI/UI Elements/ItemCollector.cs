using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
namespace Nonogram
{
    public class ItemCollector : MonoBehaviour
    {
        public GameObject diamondImage;
        public GameObject starImage;
        public GameObject starPrefab;
        public GameObject diamondPrefab;
        public CurrencyIndicator diamondIndicator;
        public CurrencyIndicator starIndicator;

        [SerializeField] GameObject panelCanvas;
        [SerializeField] private float treshold = 100f;
        [SerializeField] private float diamondTreshold = 50f;
        [SerializeField] private float starDelayTreshold = 0.2f;
        [SerializeField] private int maxDiamondImage = 30;
        [SerializeField] private int maxStarImage = 20;

        private bool diamondPass = false;
        private bool starPass = false;
        // Start is called before the first frame update

        public void RevealDiamonds(Transform transform2)
        {
            Transform transform3 = transform2.parent;
            Vector3 position = transform3.GetComponent<RectTransform>().localPosition -
                new Vector3(
                transform3.parent.GetComponent<RectTransform>().localPosition.x,
                transform3.parent.GetComponent<RectTransform>().rect.height / 2.0f,
                0);
            int tempAmount = LevelManager.instance.GetCollectedItems(CurrencyItemType.Diamond);
            int count = Mathf.Min((int)(tempAmount / diamondTreshold),maxDiamondImage);
            int index = -count / 2;
            if (tempAmount <= 0)
            {
                return;
            }
            else if(tempAmount<diamondTreshold)
            {
                count = 1;
            }
            for (int i=0;i<count;i++)
            {
                PoolItem poolItem = PoolManager.instance.GetFromPool(PoolItemType.DiamondImage, transform3);
                if (poolItem == null)
                {
                    poolItem = Instantiate(diamondPrefab).GetComponent<PoolItem>();
                    PoolManager.instance.AddToUsing(poolItem, panelCanvas.transform);
                }
                else
                {
                    poolItem.transform.parent = panelCanvas.transform;
                }
                poolItem.transform.GetComponent<RectTransform>().localPosition = new Vector3(position.x, position.y, 0);
                poolItem.gameObject.SetActive(true);
                poolItem.transform.GetComponent<RectTransform>().DOMove(
                    new Vector3(poolItem.transform.position.x + (treshold * index), poolItem.transform.position.y + treshold+(treshold)*(i%2), position.z),
                    0.5f);
                if (i == count - 1)
                {
                    diamondPass = true;
                }
                TargetDiamondToItem(poolItem.gameObject, poolItem, 1.1f, tempAmount,i);
                index++;
            }
        }

        public void RevealStars(Transform transform2)
        {
            Transform transform3 = transform2.parent;
            Vector3 position = transform3.GetComponent<RectTransform>().localPosition -
                new Vector3(
                transform3.parent.GetComponent<RectTransform>().localPosition.x,
                transform3.parent.GetComponent<RectTransform>().rect.height / 2.0f,
                0);
     
            int tempAmount = Mathf.Min(LevelManager.instance.GetCollectedItems(CurrencyItemType.Star),maxStarImage);
            if (tempAmount <= 0)
            {
                return;
            }
            for(int i= 0;i < tempAmount;i++)
            {
                PoolItem poolItem = PoolManager.instance.GetFromPool(PoolItemType.StarImage, transform3);
                if (poolItem == null)
                {
                    poolItem = Instantiate(starPrefab).GetComponent<PoolItem>();
                    PoolManager.instance.AddToUsing(poolItem, panelCanvas.transform);
                }
                else
                {
                    poolItem.transform.parent = panelCanvas.transform;
                }
                if (i >= tempAmount) { return; }
                poolItem.transform.GetComponent<RectTransform>().localPosition = new Vector3(position.x, position.y, 0);
                poolItem.gameObject.SetActive(true);
                poolItem.transform.DOScale(Vector3.one, 0.4f).From(Vector3.zero).SetDelay(0.2f);
                poolItem.transform.GetComponent<RectTransform>().DOMove(
                 new Vector3(poolItem.transform.position.x + (treshold), poolItem.transform.position.y + treshold, position.z),
                 0.5f);

                if (i == tempAmount - 1)
                {
                    starPass = true;
                }
                TargetStarToItem(poolItem.gameObject, poolItem, 1.7f + (i * starDelayTreshold), 1,i);
            }
        }

        private void TargetDiamondToItem(GameObject image,PoolItem item,float delayTime,int amount,int i)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(item.gameObject.transform.DOScale(Vector3.one * 1.5f, 2f).From(Vector3.zero).SetDelay(0.1f*i));
            sequence.Append(image.transform.DOMove(diamondImage.transform.position, delayTime));
            sequence.Append(image.transform.DOScale(0f, 0.05f));
            sequence.OnStepComplete(() => ChangeCurrencyText(CurrencyItemType.Diamond,amount,delayTime));
        }
        private void TargetStarToItem(GameObject image,PoolItem item,float delayTime,int amount,int i)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(item.gameObject.transform.DOScale(Vector3.one * 1.5f, 1f).From(Vector3.zero).SetDelay(0.15f * i));
            sequence.Append(image.transform.DOMove(starImage.transform.position, delayTime));
            sequence.Append(starImage.transform.DOScale(Vector3.one * 1.1f, 0.05f).From(Vector3.one).OnComplete(() => starImage.transform.localScale = Vector3.one));
            sequence.Append(image.transform.DOScale(0f, 0.05f));
            sequence.OnComplete(() => ChangeCurrencyText(CurrencyItemType.Star,amount,delayTime));
        }
        private void ChangeCurrencyText(CurrencyItemType type,int amount,float delayTime)
        {
            if (type == CurrencyItemType.Diamond && diamondPass)
            {
                diamondIndicator.SetCurrencyText(CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Diamond).amount - amount,
                CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Diamond).amount, delayTime);
                diamondPass = false;
            }
            if (type == CurrencyItemType.Star && starPass)
            {
                starIndicator.SetCurrencyText(CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Star).amount - amount,
                CurrencyManager.instance.GetCurrencyItem(CurrencyItemType.Star).amount, delayTime);
                starPass = false;
            }
            
        }

        private void DisActivateItem(GameObject image)
        {
            image.transform.localScale = Vector3.zero;
        }
    }

}
