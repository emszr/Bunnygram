using UnityEngine;
using System;
namespace Nonogram
{
    public enum SoundType
    {
        ButtonClick=0,
        ButtoUnClick = 1,
        CellClick=2,
        ErrorClick=3,
        WinSound=4,
        LoseSound=5,
        Music=6,
        Swipe=7,
        Horizontal=8,
        Vertical=9,
        Bomb=10,
    }

    [Serializable]
    public class Sound
    {
        public SoundType type;
        public AudioClip soundClip;

    }
    [Serializable]
    public class GameSound
    {
        public GameSound(bool onGameSound, bool onMusicSound, bool onVibration)
        {
            this.onGameSound = onGameSound;
            this.onMusicSound = onMusicSound;
            this.onVibration = onVibration;
        }
        public bool onGameSound;
        public bool onMusicSound;
        public bool onVibration;
    }
}

