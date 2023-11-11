using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Mover : MonoBehaviour
{
    [SerializeField] GameObject box;
    [SerializeField] GameObject Pos1;
    [SerializeField] GameObject Pos2;
    [SerializeField] float speed;
    [SerializeField] float moveAmount;

    float elapsedtime;
    Vector3 startPos;
    Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBeatEvent += doBeat;
        box.transform.position = Pos1.transform.position;
        startPos = Pos1.transform.position;
        endPos = Pos2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void doBeat()
    {
        elapsedtime += moveAmount;
        if (elapsedtime < 1)
        {
            box.transform.position = Vector3.Lerp(startPos, endPos, elapsedtime);
        }
        else
        {
            elapsedtime = 0;
            Vector3 temp = startPos;
            startPos = endPos;
            endPos = temp;
        }
    }
}
