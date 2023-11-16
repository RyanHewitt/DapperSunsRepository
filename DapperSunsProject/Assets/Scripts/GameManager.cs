using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject menuEndGame;
    [SerializeField] GameObject tutorialMenu;


    private float elapsedTime = 0f;
    public bool isCountingTimer;

    public AudioClip audioClip;
    AudioSource audioSource;

    float bpm;
    int lastSampledTime;

    Stack<GameObject> menuStack = new Stack<GameObject>();
    GameObject playerSpawn;
    GameObject mainmenu;

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
    }

    void Update()
    {
        if (AudioManager.instance.audioSource.isPlaying)
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

        if (playerDead)
        {
            if (Input.anyKeyDown)
            {
                Restart();
                AudioManager.instance.Unmuffle();
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        AudioManager.instance.audioSource.Pause();
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOg;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        AudioManager.instance.audioSource.UnPause();
    }

    public void PopupPause()
    {
        StatePause();
        menuStack.Push(menuPause);
        menuStack.Peek().SetActive(true);
    }

    public void PopupOptions()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(menuOptions);
        menuStack.Peek().SetActive(true);
    }

    public void PopupControls()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }
        menuStack.Push(menuControls);
        menuStack.Peek().SetActive(true);
    }

    public void PopupWin()
    {
        StatePause();
        menuStack.Push(menuWin);
        menuStack.Peek().SetActive(true);
    }
    public void PopupEndGame()
    {
        StatePause();
        menuStack.Push(menuEndGame);
        menuStack.Peek().SetActive(true);
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
    }

    public void Back()
    {
        menuStack.Peek().SetActive(false);
        menuStack.Pop();
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
    }

    public void ToMainMenu()
    {
        while (menuStack.Count > 0)
        {
            Back();
        }
        StatePause();
        SceneManager.LoadScene("MainMenu");
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

    public void SyncBeats(float _bpm)
    {
        bpm = _bpm;
        StartTimer();
        AudioManager.instance.playAudio(audioClip);
    }

    public IEnumerator FlashLines(float duration)
    {
        SpeedLines.SetActive(true);
        yield return new WaitForSeconds(duration);
        SpeedLines.SetActive(false);
    }

    public event BeatEvent OnBeatEvent;
    public event BeatEvent OnRestartEvent;
}