using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Nonogram
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ShopItem", order = 1)]
    public class ShopItem : ScriptableObject
    {
        public CurrencyItemType type;
        public int itemCost;
        public int amountToBeAdded;
        public string title;
        public string descripton;


    }
}

