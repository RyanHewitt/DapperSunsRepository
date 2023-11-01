using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject screenFlashDmg;

    public GameObject player;

    public bool isPaused;
    public bool playerDead;

    public List<Beat> beatObjects;

    float timeScaleOg;

    void Awake()
    {
        instance = this;
        timeScaleOg = Time.timeScale;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !playerDead)
        {
            if (menuActive != null)
            {
                stateUnpause();
                return;
            }
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }

        if (playerDead)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                stateUnpause();
            } 
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // For now, syncing beats here. Later, this should be called on the Audio Manager whenever a song is started
            SyncBeats(120f);
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOg;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void popupWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void popupLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void SyncBeats(float _bpm)
    {
        foreach (Beat beat in beatObjects)
        {
            beat.SetBPM(_bpm);
            beat.StartTimer();
        }
    }
}
