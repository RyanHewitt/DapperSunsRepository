using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLDisable : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_WEBGL
        gameObject.SetActive(false);
#endif
    }
}