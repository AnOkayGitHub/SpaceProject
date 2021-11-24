using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Transform itemHolder;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image bossHealthbar;
    [SerializeField] private Image playerHealthbar;
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private GameObject bossHUD;

    private void Start()
    {
        UpdateWorldData();
    }

    private void UpdateWorldData()
    {
        if (World.bossHealthbar == null)
        {
            World.bossHealthbar = bossHealthbar;
        }

        if (World.bossName == null)
        {
            World.bossName = bossName;
        }

        if (World.bossHUD == null)
        {
            World.bossHUD = bossHUD;
        }
    }

    public void UpdateHealthbar(float current, float max)
    {
        playerHealthbar.fillAmount = current / max;
    }

    public Image GetPlayerHealthbar()
    {
        return playerHealthbar;
    }

    public void UpdateCoins()
    {
        coinText.text = "x" + World.coins.ToString();
    }

    public void UpdateLevel()
    {
        levelText.text = "Floor " + World.level.ToString();
    }

    public void UpdateItems(string t, Sprite s, string d)
    {
        GameObject g = Instantiate(itemPrefab, itemHolder);
        g.GetComponent<Image>().sprite = s;
        g.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = t;
        g.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = d;
    }
}
