using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinMenu : MonoBehaviour
{
    [SerializeField] Image medal;
    [SerializeField] 

    LevelStats stats;

    void Start()
    {
        stats = GameManager.instance.FindLevelStats(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        
    }
}
