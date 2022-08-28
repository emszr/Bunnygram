using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nonogram
{
    public class ItemActionPanel : MonoBehaviour
    {
        #region SerializedFields
        [SerializeField]
        private GameObject button_prefab;
        #endregion

        public void AddButton(string buttonName,Action OnClickAction)
        {
            GameObject obj = Instantiate(button_prefab, transform);
            Button button= obj.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickAction());
            button.GetComponentInChildren<TMPro.TextMeshPro>().text = buttonName;
        }


    }

}
