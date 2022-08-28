using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Nonogram
{
    public class SaveButton : MonoBehaviour
    {
        private Button button;
        
        private void Start()
        {
            button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(TaskOnClick);
        }

        private void TaskOnClick()
        {
   //         System.SaveLoadSystem.SaveData<LevelManager>(System.SaveLoad.LevelData, LevelManager.instance);
        }

    }
}

