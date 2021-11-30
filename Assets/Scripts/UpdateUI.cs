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
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI bossHPText;
    [SerializeField] private Image bossHealthbar;
    [SerializeField] private Image playerHealthbar;
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private GameObject bossHUD;
    [SerializeField] private GameObject shopHUD;
    [SerializeField] private Animator gameOverHUD;

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

        if(World.bossHP == null)
        {
            World.bossHP = bossHPText;
        }
    }

    public void GameOver()
    {
        gameOverHUD.Play("HUDGameover", -1, 0f);
    }

    public void UpdateHealthbar(float current, float max)
    {
        playerHealthbar.fillAmount = current / max;
        playerHPText.text = current.ToString("0") + "/" + max.ToString("0");
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

    public void ToggleShopHud()
    {
        shopHUD.gameObject.SetActive(!shopHUD.activeSelf);
    }

    public void UpdateItems(string t, Sprite s, string d)
    {
        GameObject g = Instantiate(itemPrefab, itemHolder);
        g.GetComponent<Image>().sprite = s;
        g.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = t;
        g.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = d;
    }
}
