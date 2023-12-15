using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinMenu : MonoBehaviour
{
    [SerializeField] Image medal;
    [SerializeField] Image bronze;
    [SerializeField] Image silver;
    [SerializeField] Image gold;
    [SerializeField] Image diamond;
    [SerializeField] TMP_Text silverText;
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text diamondText;
    [SerializeField] TMP_Text yourTimeText;
    [SerializeField] TMP_Text bestTimeText;
    [SerializeField] Sprite[] medalSprites;

    LevelStats stats;

    int badgeIndex;

    void Awake()
    {
        stats = GameManager.instance.FindLevelStats(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        CalculateMedal();

        yourTimeText.text = GameManager.instance.ConvertTime(GameManager.instance.elapsedTime);
        bestTimeText.text = GameManager.instance.ConvertTime(stats.bestTime);
        silverText.text = GameManager.instance.ConvertTime(stats.silverTime);
        goldText.text = GameManager.instance.ConvertTime(stats.goldTime);
        diamondText.text = GameManager.instance.ConvertTime(stats.diamondTime);

        switch (badgeIndex)
        {
            case 4:
                diamond.color = Color.white;
                gold.color = Color.white;
                silver.color = Color.white;
                bronze.color = Color.white;
                break;
            case 3:
                diamond.color = Color.gray;
                gold.color = Color.white;
                silver.color = Color.white;
                bronze.color = Color.white;
                break;
            case 2:
                diamond.color = Color.gray;
                gold.color = Color.gray;
                silver.color = Color.white;
                bronze.color = Color.white;
                break;
            case 1:
                diamond.color = Color.gray;
                gold.color = Color.gray;
                silver.color = Color.gray;
                bronze.color = Color.white;
                break;
        }
    }

    void CalculateMedal()
    {
        float timer = GameManager.instance.elapsedTime;
        if (timer <= stats.diamondTime)
        {
            badgeIndex = 4;
            medal.sprite = medalSprites[badgeIndex];
        }
        else if (timer <= stats.goldTime)
        {
            badgeIndex = 3;
            medal.sprite = medalSprites[badgeIndex];
        }
        else if (timer <= stats.silverTime)
        {
            badgeIndex = 2;
            medal.sprite = medalSprites[badgeIndex];
        }
        else
        {
            badgeIndex = 1;
            medal.sprite = medalSprites[badgeIndex];
        }

        if (stats.bestTime > timer)
        {
            stats.bestTime = timer;
        }

        if (stats.badgeIndex <  badgeIndex)
        {
            stats.badgeIndex = badgeIndex;
        }
    }
}
