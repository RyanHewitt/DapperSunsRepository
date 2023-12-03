using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("----- UI -----")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject menuQuit;
    [SerializeField] GameObject SpeedLines;
    [SerializeField] GameObject grooveEdge;
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject menuEndGame;
    [SerializeField] GameObject tutorialMenu;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] GameObject optionsstart;
    [SerializeField] GameObject mainmenustart;
    [SerializeField] GameObject tutorialstart;
    [SerializeField] GameObject controlsstart;
    [SerializeField] GameObject quitstart;
    [SerializeField] GameObject pausestart;
    [SerializeField] GameObject endgamestart;
    [SerializeField] GameObject winstart;
    [SerializeField] GameObject DoubleTimePrefab;
    [SerializeField] GameObject AudioMenu;
    [SerializeField] GameObject Audiostart;
    [SerializeField] GameObject Videomenu;
    [SerializeField] GameObject Videostart;

    [SerializeField] Slider FPS;
    [SerializeField] Slider FOV;

    public bool doubleTimeActive = false;
    public AudioClip originalSong; 
    float elapsedTime = 0f;
    public bool isCountingTimer;
    private float originalBpm; 
 

    public AudioClip audioClip;
    AudioSource audioSource;

    float bpm;
    int lastSampledTime;
    int doubleTimeCount;

    Stack<GameObject> menuStack = new Stack<GameObject>();
     public Stack<GameObject> buttonStack = new Stack<GameObject>();
    GameObject playerSpawn;
    private GameObject lastSelectedButton;

    public delegate void BeatEvent();

    [Header("----- Public -----")]
    public GameObject player;
    public PlayerController playerScript;

    public bool isPaused;
    public bool playerDead;
    public bool beatWindow;

    public float beatTime;
    public float timeScaleOg;

    void Awake()
    {

        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawn = GameObject.FindWithTag("Respawn");
    }

    void Start()
    {
        audioSource = AudioManager.instance.audioSource;

        FOV.value = PlayerPrefs.GetInt("FOV", 90);
        FPS.value = PlayerPrefs.GetInt("MaxFPS", 60);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1000f);
        playerScript.sensitivity = sensitivitySlider.value;
    }

    void Update()
    {
        if (AudioManager.instance.musicPlaying && !playerDead)
        {
            CheckBeat();
        }

        if (Input.GetButtonDown("Cancel") && !playerDead)
        {
            if (menuStack.Count > 0)
            {
                if (menuStack.Peek() == menuWin)
                {
                    return;
                }
                else if (menuStack.Peek() == menuEndGame)
                {
                    return;
                }
                else
                {
                    Back();
                }
            }
            else if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                PopupPause();
            }
        }

        if (Input.GetButtonDown("Restart"))
        {
            Restart();
            AudioManager.instance.Unmuffle();

        }

        CheckTimer();
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    void CheckBeat()
    {
        float beatInterval = 60f / bpm;
        float sampledTime = (AudioManager.instance.MusicSource.timeSamples / (AudioManager.instance.MusicSource.clip.frequency * beatInterval)) - 0.25f;
        int floor = Mathf.FloorToInt(sampledTime);
        if (floor != lastSampledTime)
        {
            lastSampledTime = floor;
            if (OnBeatEvent != null)
            {
                OnBeatEvent();
                //StatePause();
            }
        }

        beatTime = sampledTime - floor;

        if (beatTime < 0.25f || beatTime > 0.75f)
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

    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioManager.instance.audioSource.Pause();
        AudioManager.instance.musicPlaying = false;
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOg;
        AudioManager.instance.audioSource.UnPause();
        AudioManager.instance.musicPlaying = true;
    }

    public void PopupPause()
    {
        StatePause();
        menuStack.Push(menuPause);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(pausestart);
        buttonStack.Push(pausestart);
    }

    public void PopupOptions()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(menuOptions);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsstart);
        buttonStack.Push(optionsstart);
    }

    public void PopupControls()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(menuControls);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(controlsstart);
        buttonStack.Push(controlsstart);
    }
    public void AudioMenuPopup()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(AudioMenu);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(Audiostart);
        buttonStack.Push(Audiostart);
    }
    public void VideoMenuPopup()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(Videomenu);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(Videostart);
        buttonStack.Push(Videostart);
    }

    public void PopupWin()
    {
        StatePause();
        menuStack.Push(menuWin);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(winstart);
        buttonStack.Push(winstart);
    }

    public void PopupEndGame()
    {
        StatePause();
        menuStack.Push(menuEndGame);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(endgamestart);
        buttonStack.Push(endgamestart);
    }

    public void PopupLose()
    {
        AudioManager.instance.Muffle();
        isPaused = true;
        menuStack.Push(menuLose);
        menuStack.Peek().SetActive(true);
    }

    public void PopupQuit()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(menuQuit);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(quitstart);
        buttonStack.Push(quitstart);
    }

    public void ToggleGrooveEdge(bool isGroove)
    {
        grooveEdge.SetActive(isGroove);
    }

    public void Back()
    {
        menuStack.Peek().SetActive(false);
        menuStack.Pop();

        if (buttonStack.Count > 0)
        {
            buttonStack.Pop(); 
        }

        if (buttonStack.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(buttonStack.Peek());
        }

        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(true);
        }
        else if (isPaused && SceneManager.GetActiveScene().name != "MainMenu")
        {
            StateUnpause();
        }
    }

    public void Restart()
    {
        elapsedTime = 0;
        playerDead = false;

        if (OnRestartEvent != null)
        {
            OnRestartEvent();
        }

        while (menuStack.Count > 0)
        {
            Back();
        }

        if (doubleTimeActive)
        {
            DeactivateDoubleTimePowerUp();
        }
    }

    public void ToMainMenu()
    {
        while (menuStack.Count > 0)
        {
            Back();
        }
        SceneManager.LoadScene("MainMenu");
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        isPaused = true;
    }

    public void AppQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void TutorialQuestion()
    {
        while (menuStack.Count > 0)
        {
            Back();
        }
        menuStack.Push(tutorialMenu);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(tutorialstart);
        buttonStack.Push(tutorialstart);
    }

    public void ToTutorial()
    {
        while (menuStack.Count > 0)
        {
            Back();
        }
        SceneManager.LoadScene("Tutorial");
        StateUnpause();
    }

    public void RejectTutorial()
    {
        while (menuStack.Count > 0)
        {
            Back();
        }
        SceneManager.LoadScene("Level 1");
        StateUnpause();
    }


    public void CheckTimer()
    {
        if (!isPaused && isCountingTimer && timerText != null)
        {
            elapsedTime += Time.deltaTime;

            int minutes = (int)elapsedTime / 60;
            int seconds = (int)elapsedTime % 60;
            int milliseconds = (int)((elapsedTime * 100) % 100);

            string timerString = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

            timerText.text = timerString;
        }
    }

    public void StartTimer()
    {
        isCountingTimer = true;
        elapsedTime = 0;
    }

    public void SyncBeats(float _bpm)
    {
        originalBpm = bpm;
        bpm = _bpm;
        if (!isCountingTimer)
        {
            StartTimer();
        }
        AudioManager.instance.ChangeSong(audioClip);
    }

    public IEnumerator FlashLines(float duration)
    {
        SpeedLines.SetActive(true);
        yield return new WaitForSeconds(duration);
        SpeedLines.SetActive(false);
    }

    public void SetSensitivity()
    {
        playerScript.sensitivity = sensitivitySlider.value;
        PlayerPrefs.SetFloat("Sensitivity", playerScript.sensitivity);
    }

    public void ActivateDoubleTimePowerUp(AudioClip doubleTimeSong, float doubleBpm, float duration)
    {
        if (!doubleTimeActive)
        {
            doubleTimeCount++;

            originalSong = AudioManager.instance.audioSource.clip; // Store the current song
            audioClip = doubleTimeSong; // Set the double-time song
            AudioManager.instance.ChangeSong(audioClip); // Change to the double-time song

            doubleTimeActive = true;
            SyncBeats(doubleBpm);
            
            // Start a coroutine to end double-time after its duration
            StartCoroutine(EndDoubleTimeAfterDuration(duration));
        }
    }

    // Coroutine to end double-time
    public IEnumerator EndDoubleTimeAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (doubleTimeCount == 1)
        {
            doubleTimeCount = 0;
            DeactivateDoubleTimePowerUp();
        }
        else
        {
            doubleTimeCount--;
        }
    }

    // Method to deactivate double-time
    public void DeactivateDoubleTimePowerUp()
    {
        if (doubleTimeActive)
        {
            doubleTimeActive = false;
            audioClip = originalSong;
            SyncBeats(originalBpm);
        }
    }
    public void Set1080P()
    {

        //List<int> widths = new List<int>() { 1280, 1920, 2560, 3840, 5120 };
        //List<int> heights = new List<int>() { 720, 1080, 1440, 1440, 1440 };
        //bool fullscreen = Screen.fullScreen;
        //int width = widths[index];
        //int height = heights[index];
        //Screen.SetResolution(width, height, fullscreen);
        Screen.SetResolution(1920, 1080, true);
    }
    public void Set1440P()
    {
        Screen.SetResolution(2560, 1440, true);
    }

    public void SetFullscreen()
    {
        //if (Fullscreen.enabled)
        //{
        //    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        //}
        //else
        //{
        //    Screen.fullScreenMode = FullScreenMode.Windowed;
        //}
    }

    public void SetmFPS()
    {
        Application.targetFrameRate = (int)FPS.value;
        PlayerPrefs.SetInt("MaxFPS", (int)FPS.value);
    }

    public void SetFOV()
    {
        Camera.main.fieldOfView = FOV.value;
        PlayerPrefs.SetInt("FOV", (int)FOV.value);
    }

    public event BeatEvent OnBeatEvent;
    public event BeatEvent OnRestartEvent;
}