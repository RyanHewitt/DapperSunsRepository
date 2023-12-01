using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public void OpenLevel(int levelid)
    {
        string levelname = "Level" + levelid;
        SceneManager.LoadScene(levelname);
    }

}
