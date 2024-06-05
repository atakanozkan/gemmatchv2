using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace BlockShift
{
    
    [Flags]
    public enum GameState
    {
        Default = 0,
        Playing = 1,
        PowerUp = 2,
        Pause = 4,
        Lose = 8,
        Win = 16,
        Spinning=32,
        Shop =64,
        Filling =128,
        Falling =256,
        Loading = 512
    }
    
}


