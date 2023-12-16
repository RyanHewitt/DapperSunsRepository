using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] float duration;
    [SerializeField] float targetScale;
    [SerializeField] float speed;
    [SerializeField] float fadeSpeed;

    float elapsedTime = 0;

    void Awake()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        elapsedTime += speed * Time.deltaTime;
        transform.localScale = Vector3.one * Mathf.Lerp(1f, targetScale, elapsedTime);

        Color color = model.material.color;
        color.a = Mathf.Lerp(color.a, 0f, fadeSpeed * Time.deltaTime);
        model.material.color = color;
    }
}
