using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float destroyChance = 20;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        Color c = Random.ColorHSV(0.15f, 0.6f, 0.25f, 0.75f, 0.3f, 0.6f);
        spriteRenderer.color = c;
        RandomDestroy();
    }

    private void RandomDestroy()
    {
        float rand = Random.Range(0, 101);

        if(rand <= destroyChance)
        {
            Destroy(gameObject, 0f);
        }
    }
}
