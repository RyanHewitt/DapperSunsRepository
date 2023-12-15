using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class LevelStats : ScriptableObject
{
    public int badgeIndex;
    public float silverTime;
    public float goldTime;
    public float diamondTime;

    private float _bestTime = 9999;
    public float bestTime
    {
        get { return _bestTime; }
        set { _bestTime = value; }
    }
}