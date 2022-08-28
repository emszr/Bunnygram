using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Nonogram
{
    public class LevelButton : MonoBehaviour
    {
        public List<Image> starList;
        public Button button;
        public Level loadedLevel;
        public int indexLevel;
        public bool levelButtonUnLocked=false;
        public TextMeshProUGUI textMeshPro;
        public Image image;

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }
    }

}
