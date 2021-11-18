using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeItem : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private string[] names;
    private SpriteRenderer spriteRenderer;
    private string itemName;
    private int id;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BecomeNewItem();
    }

    private void BecomeNewItem()
    {
        id = Random.Range(0, sprites.Length);
        spriteRenderer.sprite = sprites[id];
        itemName = names[id];

        if(World.items.ContainsKey(itemName))
        {
            BecomeNewItem();
        }
    }

    public string GetName()
    {
        return itemName;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }
}
