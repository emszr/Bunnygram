using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Nonogram
{
    public class LevelInfo : MonoBehaviour
    {
        public TextMeshProUGUI textMesh;
        // Start is called before the first frame update
        void Start()
        {
            textMesh.text = "LEVEL " + (LevelManager.instance.currentLevel.levelCount).ToString();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
