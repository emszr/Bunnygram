using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Nonogram
{
    public class LosePanel : MonoBehaviour
    {
        public void PlayAgain()
        {
            LevelManager.instance.LoadRefereshedLevel();
            DOTween.KillAll();
            SceneManager.LoadScene(1);
        }

        public void MainMenuButtonFunction()
        {
            LevelManager.instance.SaveLevel();
            DOTween.KillAll();
            SceneManager.LoadScene(0);
        }
        public void OnEnable()
        {
            EnableAnimation();
        }

        public void EnableAnimation()
        {
            SoundManager.instance.PlaySound(SoundType.LoseSound);
            gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

    }

}
