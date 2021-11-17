using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float floatRange = 2f;
    [SerializeField] private float floatSpeed = 1f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, World.player.transform.position) < floatRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, World.player.transform.position, floatSpeed * Time.deltaTime);
        }
    }
}
