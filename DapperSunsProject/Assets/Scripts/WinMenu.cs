using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    LevelStats stats;

    void Start()
    {
        stats = GameManager.instance.FindLevelStats(SceneManager.GetActiveScene().name);
    }
}
