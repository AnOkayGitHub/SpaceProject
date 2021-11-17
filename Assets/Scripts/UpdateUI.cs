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

    public void UpdateCoins()
    {
        coinText.text = "x" + World.coins.ToString();
    }

    public void UpdateLevel()
    {
        levelText.text = World.level.ToString();
    }

    public void UpdateItems(string t, Sprite s)
    {
        GameObject g = Instantiate(itemPrefab, itemHolder);
        g.GetComponent<Image>().sprite = s;
        g.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = t;
    }
}
