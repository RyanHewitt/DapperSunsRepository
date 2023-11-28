using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    private GameObject lastSelectedButton;
    [SerializeField] GameObject DefaultButton;
    public void Start()
    {
        GameManager.instance.isPaused = true;
        GameManager.instance.buttonStack.Push(DefaultButton);
    }
    public void Update()
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