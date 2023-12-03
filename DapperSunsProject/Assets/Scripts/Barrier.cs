using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnEnable()
    {
        SuperHeavy.OnBossDefeated += DisableBarrier;
    }

    private void OnDisable()
    {
        SuperHeavy.OnBossDefeated -= DisableBarrier;
    }

    private void DisableBarrier()
    {
        // Disable or destroy the barrier
        gameObject.SetActive(false);
    }
}