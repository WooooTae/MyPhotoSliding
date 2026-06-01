using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    Easy,
    Normal,
    Hard
}

[System.Serializable]
public class PuzzleSettings
{
    public Level level;
    public float bgmSound;
    public float sfxSound;
}
