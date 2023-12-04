using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    public float timer;
    [SerializeField] Image bronzebadge;
    [SerializeField] Image silverbadge;
    [SerializeField] Image goldbadge;
    [SerializeField] Image diamondbadge;
    [SerializeField] float bronzetime;
    [SerializeField] float silvertime;
    [SerializeField] float goldtime;
    [SerializeField] float diamondtime;

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
            if(timer < diamondtime)
            {
                diamondbadge.enabled = true;
            }
            else if(timer < goldtime)
            {
                goldbadge.enabled = true;
            }
            else if(timer < silvertime)
            {
                silverbadge.enabled = true;
            }
            else if(timer > bronzetime)
            {
                bronzebadge.enabled = true;
            }
            PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + " Time", timer);
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