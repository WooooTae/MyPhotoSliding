using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    Easy,
    Medium,
    Hard
}

[System.Serializable]
public class PuzzleSettings
{
    public Level level;
    public bool isSound;
}
