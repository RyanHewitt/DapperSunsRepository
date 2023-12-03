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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.instance.StateUnpause();
    }
    public void TutorialMenu()
    {
        GameManager.instance.TutorialQuestion();
    }
    public void YesTutorial()
    {
        GameManager.instance.ToTutorial();
    }
    public void NoTutorial()
    {
        GameManager.instance.RejectTutorial();
    }
    public void AudioMenu()
    {
        GameManager.instance.AudioMenuPopup();
    }
    public void VideoMenu()
    {
        GameManager.instance.VideoMenuPopup();
    }
}