using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nonogram
{
    public class SettingsPanel : MonoBehaviour
    {

        #region OffImages
        [SerializeField] private GameObject globalOffImage;
        [SerializeField] private GameObject musicOffImage;
        [SerializeField] private GameObject vibrationOffImage;
        #endregion

        private void Start()
        {
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            GameSound sound = SoundManager.instance.gameSound;
            if (!sound.onGameSound)
            {
                globalOffImage.SetActive(true);
                //musicOffImage.SetActive(true);
            }
            if(!sound.onMusicSound)
            {
                 musicOffImage.SetActive(true); 
            }
            
            if (!sound.onVibration) { vibrationOffImage.SetActive(true); }
        }
        public void ChangeGloablSound(GameObject offImage)
        {
            bool onGlobalSound = !SoundManager.instance.gameSound.onGameSound;
            if (!onGlobalSound)
            {
                offImage.SetActive(true);
                SoundManager.instance.gameSound.onGameSound = onGlobalSound;
            }
            else
            {
                SoundManager.instance.gameSound.onGameSound = onGlobalSound;
                SoundManager.instance.PlaySound(SoundType.ButtonClick);
                offImage.SetActive(false);
            }
        }
        public void ChangeMusicSound(GameObject offImage)
        {
            
            bool onMusicSound = !SoundManager.instance.gameSound.onMusicSound;
            if (!onMusicSound)
            {
                SoundManager.instance.gameSound.onMusicSound = onMusicSound;
                offImage.SetActive(true);
                SoundManager.instance.CloseMusic();
            }
            else
            {
                SoundManager.instance.gameSound.onMusicSound = onMusicSound;
                SoundManager.instance.PlaySound(SoundType.ButtonClick);
                offImage.SetActive(false);
                SoundManager.instance.OpenMusic();
            }
        }
        public void ChangeVibration(GameObject offImage)
        {
            bool onVibration = !SoundManager.instance.gameSound.onVibration;
            if (!onVibration)
            {
                SoundManager.instance.PlaySound(SoundType.ButtonClick);
                offImage.SetActive(true);
            }
            else
            {
                SoundManager.instance.PlaySound(SoundType.ButtonClick);
                offImage.SetActive(false);
            }
            SoundManager.instance.gameSound.onVibration = onVibration;
        }

    }
}

