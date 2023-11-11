using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Mover : MonoBehaviour
{
    [SerializeField] GameObject box;
    [SerializeField] GameObject[] positions;
    [SerializeField] float speed;
    [SerializeField] float moveAmount;

    int nextIndex;
    float elapsedtime;
    Vector3 startPos;
    Vector3 endPos;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;
        
        nextIndex = 1;

        box.transform.position = positions[nextIndex - 1].transform.position;
        startPos = positions[nextIndex - 1].transform.position;
        endPos = positions[nextIndex].transform.position;
    }

    void Update()
    {
        elapsedtime += speed * Time.deltaTime;
        box.transform.position = Vector3.Lerp(startPos, endPos, elapsedtime);
    }

    void DoBeat()
    {
        elapsedtime = 0;

        if (nextIndex + 1 >= positions.Length)
        {
            nextIndex = 0;
            startPos = endPos;
            endPos = positions[nextIndex].transform.position;
        }
        else
        {
            nextIndex++;
            startPos = endPos;
            endPos = positions[nextIndex].transform.position;
        }
    }
}
