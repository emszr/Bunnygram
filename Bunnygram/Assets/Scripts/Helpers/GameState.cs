using System;

namespace Nonogram {

    [Flags]
    public enum GameState
    {
        Default = 0,
        Playing = 1,
        PowerUp = 2,
        Pause = 4,
        Lose = 8,
        Win = 16,
        PowerUpAnimation=32,
        Shop =64,
    }
}
