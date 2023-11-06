using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempTimer : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    private float elapsedTime = 0f;


    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        int minutes = (int)elapsedTime / 60;
        int seconds = (int)elapsedTime % 60;
        int milliseconds = (int)((elapsedTime * 1000) % 1000);

        string timerString = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

        timerText.text = timerString;
    }
}
