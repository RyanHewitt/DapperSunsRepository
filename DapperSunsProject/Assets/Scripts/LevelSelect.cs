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

    void Start()
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

        for (int i = 0;i < medalButtonImages.Length; i++)
        {
            int badgeIndex = PlayerPrefs.GetInt("Level " + (i + 1) + " badgeIndex", 0);
            medalButtonImages[i].sprite = medalSprites[badgeIndex];
        }
    }

    public void OpenLevel(int levelid)
    {
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
