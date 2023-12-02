using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] Slider FPS;

    List<int> widths = new List<int>() { 1280, 1920, 2560, 3840, 5120 };
    List<int> heights = new List<int>() { 720, 1080, 1440, 1440, 1440 };

    public void SetScreenSize(int index)
    {
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetmFPS()
    {
        Application.targetFrameRate = (int)FPS.value;
        PlayerPrefs.SetFloat("MaxFPS", FPS.value);
    }
}
