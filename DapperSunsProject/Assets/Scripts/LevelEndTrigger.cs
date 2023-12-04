using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    float timer;
    [SerializeField] Image bronzebadge;
    [SerializeField] Image silverbadge;
    [SerializeField] Image goldbadge;
    [SerializeField] Image diamondbadge;

    void OnTriggerEnter(Collider other)
    {
        diamondbadge.enabled = false;
        goldbadge.enabled = false;
        silverbadge.enabled = false;
        bronzebadge.enabled = false;

        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameManager.instance.PopupWin();
            timer = GameManager.instance.elapsedTime;
            if(timer < 15.000000)
            {
                diamondbadge.enabled = true;
            }
            else if(timer < 30.000000)
            {
                goldbadge.enabled = true;
            }
            else if(timer < 45.000000)
            {
                silverbadge.enabled = true;
            }
            else if(timer > 60.000000)
            {
                bronzebadge.enabled = true;
            }
        }

        UnlockNewLevel();
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();

        }
    }
}