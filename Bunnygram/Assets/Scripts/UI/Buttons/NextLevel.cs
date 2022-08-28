using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Nonogram
{
    public class NextLevel : MonoBehaviour
    {
        Button button;
        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(GameManager.instance.LoadNextLevelScene);
        }
    }

}
