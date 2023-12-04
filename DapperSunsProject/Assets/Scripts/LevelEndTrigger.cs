using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] Sprite[] medalSprites;

    [SerializeField] float silverTime;
    [SerializeField] float goldTime;
    [SerializeField] float diamondTime;

    Image medal;

    float timer;
    int badgeIndex;

    void Start()
    {
        medal = GameManager.instance.winMedalImage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameManager.instance.PopupWin();
            timer = GameManager.instance.elapsedTime;
            if(timer <= diamondTime)
            {
                badgeIndex = 4;
                medal.sprite = medalSprites[badgeIndex];
            }
            else if(timer <= goldTime)
            {
                badgeIndex = 3;
                medal.sprite = medalSprites[badgeIndex];
            }
            else if(timer <= silverTime)
            {
                badgeIndex = 2;
                medal.sprite = medalSprites[badgeIndex];
            }
            else
            {
                badgeIndex = 1;
                medal.sprite = medalSprites[badgeIndex];
            }

            if (PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + " Time", 0) > timer)
            {
                PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + " Time", timer);
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + " badgeIndex", badgeIndex); 
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
        }
    }
}