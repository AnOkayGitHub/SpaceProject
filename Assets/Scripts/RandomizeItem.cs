using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeItem : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private string[] names;
    private SpriteRenderer spriteRenderer;
    private string name;
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
        name = names[id];

        if(World.items.ContainsKey(name))
        {
            BecomeNewItem();
        }
    }

    public string GetName()
    {
        return name;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }
}
