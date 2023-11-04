using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] AudioClip audioClip;

    float timeScaleOg;

    public GameObject player;
    public GameObject playerSpawn;
    public GameObject menuActive;

    public bool isPaused;
    public bool playerDead;

    public List<Beat> beatObjects;

    void Awake()
    {
        instance = this;
        timeScaleOg = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerSpawn = GameObject.FindWithTag("Respawn");
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !playerDead)
        {
            if (menuActive == menuPause)
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
    }

    public GameObject GetPlayerSpawn()
    {
        return playerSpawn;
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        AudioManager.instance.pauseUnpauseAudio();
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOg;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        AudioManager.instance.pauseUnpauseAudio();
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
        AudioManager.instance.playAudio(audioClip);
    }
}