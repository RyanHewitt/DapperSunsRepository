using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has quit the game");
    }
}
