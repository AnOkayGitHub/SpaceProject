using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    public void UpdateValues()
    {
        coinText.text = "x" + World.coins.ToString();
    }
}
