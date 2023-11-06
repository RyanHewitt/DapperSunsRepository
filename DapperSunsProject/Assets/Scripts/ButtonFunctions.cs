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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Level 2");
        GameManager.instance.stateUnpause();
    }
}
