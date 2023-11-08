using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("----- UI -----")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text timerText;

    private float elapsedTime = 0f;
    public bool isCountingTimer;

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
    public bool beatWindow;

    void Awake()
    {
        instance = this;
        timeScaleOg = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerSpawn = GameObject.FindWithTag("Respawn");
    }

    void Start()
    {
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
                Restart();
            }
        }

        CheckTimer();
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
        if (sampledTime - floor < 0.25f ||  sampledTime - floor > 0.75f)
        {
            beatWindow = true;
        }
        else
        {
            beatWindow = false;
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

    public void CheckTimer()
    {
        if (isCountingTimer)
        {
            elapsedTime += Time.deltaTime;

            int minutes = (int)elapsedTime / 60;
            int seconds = (int)elapsedTime % 60;
            int milliseconds = (int)((elapsedTime * 1000) % 1000);

            string timerString = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

            timerText.text = timerString;
        }
    }

    public void StartTimer()
    {
        isCountingTimer = true;
        elapsedTime = 0;
    }

    public void Restart()
    {
        playerDead = false;
        if (OnRestartEvent != null)
        {
            OnRestartEvent();
        }
        stateUnpause();
    }

    public void SyncBeats(float _bpm)
    {
        bpm = _bpm;
        StartTimer();
        AudioManager.instance.playAudio(audioClip);
    }

    public event BeatEvent OnBeatEvent;
    public event BeatEvent OnRestartEvent;
}