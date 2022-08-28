using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nonogram{

    public class BackButton : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => LevelManager.instance.LoadMainMenu());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}
