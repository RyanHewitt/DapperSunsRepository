using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void Restart()
    {
        GameManager.instance.Restart();
    }

    public void Options()
    {
        GameManager.instance.popupOptions();
    }

    public void Controls()
    {
        GameManager.instance.popupControls();
    }

    public void BackOptions()
    {
        GameManager.instance.BackButtonOptions();
    }

    public void BackControls()
    {
        GameManager.instance.BackButtonControls();
    }
    public void BackMenu()
    {
        GameManager.instance.BackButtonMenu();
    }

    public void Quit()
    {
        GameManager.instance.QuitMenu();
    }

    public void MainMenu()
    {
        GameManager.instance.MainMenuQuit();
    }
    public void GameQuit()
    {
        GameManager.instance.AppQuit();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.instance.stateUnpause();
    }
}
