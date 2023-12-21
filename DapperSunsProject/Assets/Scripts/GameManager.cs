using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

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
    [SerializeField] TMP_Text countdownText;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] GameObject optionsstart;
    [SerializeField] GameObject mainmenustart;
    [SerializeField] GameObject controlsstart;
    [SerializeField] GameObject quitstart;
    [SerializeField] GameObject pausestart;
    [SerializeField] GameObject winstart;
    [SerializeField] GameObject DoubleTimePrefab;
    [SerializeField] GameObject AudioMenu;
    [SerializeField] GameObject Audiostart;
    [SerializeField] GameObject Videomenu;
    [SerializeField] GameObject Videostart;
    [SerializeField] GameObject levelSelectMenu;
    [SerializeField] GameObject levelSelectStart;
    [SerializeField] GameObject menuCredits;
    [SerializeField] GameObject menuCreditsBack;
    [SerializeField] GameObject Clearsavemenu;
    [SerializeField] GameObject clearsavemenustart;
    [SerializeField] Slider FPS;
    [SerializeField] Slider FOV;
    [SerializeField] TMP_Dropdown res;

    [Header("----- Stats -----")]
    [SerializeField] int countdownLength;

    [Header("----- Public -----")]
    public GameObject player;
    public PlayerController playerScript;
    public List<LevelStats> levelStats = new List<LevelStats>();

    public bool doubleTimeActive = false;
    public AudioClip originalSong;
    public float ogSongTime;
    public float elapsedTime = 0f;
    public bool isCountingTimer;
    float originalBpm;
    public bool isCountdownActive;
    int countdownNumber;
    public AudioClip audioClip;

    float bpm;
    int lastSampledTime;
    int doubleTimeCount;

    Stack<GameObject> menuStack = new Stack<GameObject>();
    public Stack<GameObject> buttonStack = new Stack<GameObject>();
    GameObject playerSpawn;
    GameObject lastSelectedButton;

    public Image winMedalImage;

    public bool isPaused;
    public bool playerDead;
    public bool beatWindow;

    public float beatTime;
    public float timeScaleOg;

    public delegate void BeatEvent();

    void Awake()
    {

        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawn = GameObject.FindWithTag("Respawn");
    }

    void Start()
    {
#if !UNITY_WEBGL
        Screen.SetResolution(PlayerPrefs.GetInt("ResolutionWidth", 1920), PlayerPrefs.GetInt("ResolutionHeight", 1080), (FullScreenMode)PlayerPrefs.GetInt("fullscreen", 1)); 
        res.value = PlayerPrefs.GetInt("ResolutionIndex", 1);
#endif

        FOV.value = PlayerPrefs.GetInt("FOV", 90);
        FPS.value = PlayerPrefs.GetInt("MaxFPS", 60);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1000f);
        playerScript.sensitivity = sensitivitySlider.value;

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Restart();
        }
    }

    void Update()
    {
        if (AudioManager.instance.musicPlaying && !playerDead)
        {
            CheckBeat();
        }

        if (Input.GetButtonDown("Cancel") && !playerDead && !isCountdownActive)
        {
            if (menuStack.Count > 0)
            {
                if (menuStack.Peek() == menuWin)
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

        if (Input.GetButtonDown("Restart") && !isCountdownActive)
        {
            Restart();
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

        float sampledTime = (AudioManager.instance.MusicSource.timeSamples / (AudioManager.instance.MusicSource.clip.frequency * beatInterval)) - 0.25f; // GUN
        int floor = Mathf.FloorToInt(sampledTime);
        if (floor != lastSampledTime)
        {
            lastSampledTime = floor;
            if (OnBeatEvent != null)
            {
                OnBeatEvent();
                DoBeat();
                //StatePause();
            }
        }

        beatTime = sampledTime - floor;

        if (!doubleTimeActive)
        {
            if (beatTime < 0.25f || beatTime > 0.75f)
            {
                beatWindow = true;
            }
            else
            {
                beatWindow = false;
            }
        }
        else
        {
            if (beatTime < 0.4 || beatTime > 0.6f)
            {
                beatWindow = true;
            }
            else
            {
                beatWindow = false;
            }
        }
    }

    void DoBeat()
    {
        if (isCountdownActive)
        {
            if (countdownNumber > 0)
            {
                countdownNumber--;
            }
            else
            {
                isPaused = false;
                isCountdownActive = false;
                countdownText.text = " ";
            }
        }
    }

    public GameObject GetPlayerSpawn()
    {
        return playerSpawn;
    }

    public void StatePause()
    {
        AudioManager.instance.MuffleQuick();
        isPaused = true;
        Time.timeScale = 0f;
        // AudioManager.instance.audioSource.Pause();
        AudioManager.instance.musicPlaying = false;
    }

    public void StateUnpause()
    {
        AudioManager.instance.Unmuffle();
        isPaused = false;
        Time.timeScale = timeScaleOg;
        // AudioManager.instance.audioSource.UnPause();
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

    public void PopupLevelSelect()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(levelSelectMenu);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(levelSelectStart);
        buttonStack.Push(levelSelectStart);
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
        if (grooveEdge != null)
        {
            grooveEdge.SetActive(isGroove);
        }
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
    public void ClearSave()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(Clearsavemenu);
        menuStack.Peek().SetActive(true);
        EventSystem.current.SetSelectedGameObject(clearsavemenustart);
        buttonStack.Push(clearsavemenustart);
    }

    public void Restart()
    {
        AudioManager.instance.Unmuffle();

        elapsedTime = 0;
        CheckTimer();

        playerDead = false;

        while (menuStack.Count > 0)
        {
            Back();
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            StartCountdown();

            if (OnRestartEvent != null)
            {
                OnRestartEvent();
            }
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
        PlayerPrefs.SetFloat("ogSongTime", AudioManager.instance.MusicSource.time);
        PlayerPrefs.SetString("ogSongClip", AudioManager.instance.MusicSource.clip.name);
        SceneManager.LoadScene("MainMenu");
        isPaused = true;
    }

    public void AppQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CheckTimer()
    {
        if (!isPaused && isCountingTimer && timerText != null)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = ConvertTime(elapsedTime);
        }
    }

    public string ConvertTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        int milliseconds = (int)((time * 100) % 100);

        string timerString = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

        return timerString;
    }

    public void StartTimer()
    {
        isCountingTimer = true;
        elapsedTime = 0;
    }

    public void SyncBeats(float _bpm, float time = 0f)
    {
        originalBpm = bpm;
        bpm = _bpm;

        if (!isCountingTimer)
        {
            StartTimer();
        }
        AudioManager.instance.ChangeSong(audioClip, time);
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

            ogSongTime = AudioManager.instance.MusicSource.time;
            originalSong = AudioManager.instance.MusicSource.clip; // Store the current song
            audioClip = doubleTimeSong; // Set the double-time song

            doubleTimeActive = true;
            SyncBeats(doubleBpm);

            // Start a coroutine to end double-time after its duration
            StartCoroutine(EndDoubleTimeAfterDuration(duration));
        }
    }

    public IEnumerator EndDoubleTimeAfterDuration(float duration) // Coroutine to end double-time
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

    public void DeactivateDoubleTimePowerUp() // Method to deactivate double-time
    {
        if (doubleTimeActive)
        {
            doubleTimeActive = false;
            audioClip = originalSong;
            SyncBeats(originalBpm, ogSongTime);
        }
    }

    public void SetResolution(int index)
    {
        List<int> widths = new List<int>() { 1280, 1920, 2560, 3840, 5120 };
        List<int> heights = new List<int>() { 720, 1080, 1440, 1440, 1440 };
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("fullscreen", (fullscreen ? 1 : 0));
    }

    public void SetmFPS()
    {
        Application.targetFrameRate = (int)FPS.value;
        PlayerPrefs.SetInt("MaxFPS", (int)FPS.value);
        QualitySettings.vSyncCount = 0;
    }

    public void SetFOV()
    {
        Camera.main.fieldOfView = FOV.value;
        PlayerPrefs.SetInt("FOV", (int)FOV.value);
    }

    public void StartCountdown()
    {
        isPaused = true;
        isCountdownActive = true;
        countdownNumber = countdownLength;
        countdownText.text = "BOOP-IT!";
    }

    public void CreditsMenu()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(menuCredits);
        menuCredits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(menuCreditsBack);
        buttonStack.Push(menuCreditsBack);
    }

    public LevelStats FindLevelStats(string _name)
    {
        LevelStats result = levelStats.Find(level => level.name == _name);
        return result;
    }

    public void ClearStats()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        for(int i = 0; i < levelStats.Count; ++i)
        {
            levelStats[i].bestTime = 9999;
            levelStats[i].badgeIndex = 0;
        }
    }

    public void ClearStatsPlusBack()
    {
        ClearStats();
        Back();
    }

    public event BeatEvent OnBeatEvent;
    public event BeatEvent OnRestartEvent;
}