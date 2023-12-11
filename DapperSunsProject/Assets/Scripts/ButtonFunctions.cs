using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Restart()
    {
        GameManager.instance.Restart();
    }

    public void Options()
    {
        GameManager.instance.PopupOptions();
    }

    public void Controls()
    {
        GameManager.instance.PopupControls();
    }

    public void Back()
    {
        GameManager.instance.Back();
    }

    public void Quit()
    {
        GameManager.instance.PopupQuit();
    }

    public void GameQuit()
    {
        GameManager.instance.AppQuit();
    }

    public void ToMainMenu()
    {
        GameManager.instance.ToMainMenu();
    }

    public void NextLevel()
    {
        PlayerPrefs.SetFloat("ogSongTime", AudioManager.instance.MusicSource.time);
        PlayerPrefs.SetString("ogSongClip", AudioManager.instance.MusicSource.clip.name);
        if (SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            GameManager.instance.StateUnpause();
        }
        else
        {
            GameManager.instance.ToMainMenu();
        }
    }

    public void AudioMenu()
    {
        GameManager.instance.AudioMenuPopup();
    }

    public void VideoMenu()
    {
        GameManager.instance.VideoMenuPopup();
    }

    public void Credits()
    {
        GameManager.instance.CreditsMenu();
    }
}