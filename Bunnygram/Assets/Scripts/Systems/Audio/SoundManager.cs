using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{
    public class SoundManager : Singleton<SoundManager>
    {
        public AudioSource simpleSoundSource;
        public AudioSource musicSoundSource;
        public GameSound gameSound;
        public Sound sound;
        [SerializeField] private List<Sound> soundList;
        private Dictionary<SoundType,Sound> dictionary;

        private void Start()
        {
            gameSound = LoadGameSound();
            dictionary = new Dictionary<SoundType, Sound>();
            InitializeSounds();
            gameObject.name = "AudioManager";
        }

        private void InitializeSounds()
        {
            foreach (Sound sound in soundList)
            {
                dictionary.Add(sound.type, sound);
            }
            if (gameSound.onMusicSound)
            {
                OpenMusic();
            }
            else
            {
                CloseMusic();
            }
        }

        public void PlaySound(SoundType type)
        {
            Sound sound;
            dictionary.TryGetValue(type, out sound);
            if (sound != null)
            {
                this.sound = sound;
                AssingSound();
                simpleSoundSource.Play();
            }

        }

        public void CloseMusic()
        {
            musicSoundSource.Stop();
        }

        public void OpenMusic()
        {
            musicSoundSource.Play();
        }

        private void AssingSound()
        {
            simpleSoundSource.clip = sound.soundClip;
            if (!gameSound.onGameSound)
            {
                simpleSoundSource.volume = 0f;
            }
            else
            {
                if (sound.type == SoundType.Music)
                {
                    if (gameSound.onMusicSound)
                    {
                        simpleSoundSource.volume = 1f;
                    }
                    else
                    {
                        simpleSoundSource.volume = 0f;
                    }
                }
                else
                {
                    simpleSoundSource.volume = 1f;
                }
            }
        }


        public void SaveGameSound()
        {
            string json = JsonUtility.ToJson(gameSound, false);
            if (json != null)
            {
                PlayerPrefs.SetString("Audio", json);
                PlayerPrefs.Save();
            }
        }

        public GameSound LoadGameSound()
        {
            string json = PlayerPrefs.GetString("Audio");
            GameSound sound = JsonUtility.FromJson<GameSound>(json);
            if (sound != null)
            {
                GameSound gameSound = new GameSound(sound.onGameSound, sound.onMusicSound, sound.onVibration);
                return gameSound;
            }
            else
            {
                return new GameSound(true, true, true);
            }
        }
        private void OnApplicationQuit()
        {
            SaveGameSound();
        }
    }

}
