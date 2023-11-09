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
        
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.instance.stateUnpause();
    }
}
