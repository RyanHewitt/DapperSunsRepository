using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipSplash : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
