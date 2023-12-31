using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] Image[] medalButtonImages;
    [SerializeField] Sprite[] medalSprites;
    [SerializeField] GameObject levelButtons;

    GameObject lastSelectedButton;

    private void OnEnable()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < buttons.Length; i++) // This is bad you should just do one for loop
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
        }

        for (int i = 0; i < medalButtonImages.Length; i++)
        {
            int badgeIndex = GameManager.instance.FindLevelStats("Level " + (i + 1)).badgeIndex;
            medalButtonImages[i].sprite = medalSprites[badgeIndex];
        }
    }

    public void OpenLevel(int levelid)
    {
        PlayerPrefs.SetFloat("ogSongTime", AudioManager.instance.MusicSource.time);
        PlayerPrefs.SetString("ogSongClip", AudioManager.instance.MusicSource.clip.name);
        SceneManager.LoadScene(levelid);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

}
