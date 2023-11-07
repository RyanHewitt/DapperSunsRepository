using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("----- UI -----")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public AudioClip audioClip;
    AudioSource audioSource;

    float bpm;
    int lastSampledTime;

    float timeScaleOg;

    public delegate void BeatEvent();

    [Header("----- Public -----")]
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
        audioSource = AudioManager.instance.audioSource;
    }

    void Update()
    {
        if (AudioManager.instance.isPlaying)
        {
            CheckBeat(); 
        }

        if (Input.GetButtonDown("Cancel") && !playerDead)
        {
            if (menuActive == menuPause)
            {
                stateUnpause();
                return;
            }
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
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

    void CheckBeat()
    {
        float beatInterval = 60f / bpm;
        float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * beatInterval);
        int floor = Mathf.FloorToInt(sampledTime);
        if (floor != lastSampledTime)
        {
            lastSampledTime = floor;
            if (OnBeatEvent != null)
            {
                OnBeatEvent(); 
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
        bpm = _bpm;
        foreach (Beat beat in beatObjects)
        {
            beat.SetBPM(_bpm);
            beat.StartTimer();
        }
        AudioManager.instance.playAudio(audioClip);
    }

    public event BeatEvent OnBeatEvent;
}