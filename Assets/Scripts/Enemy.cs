using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyState currentState;
    [SerializeField] private float moveSpeed;

    private enum EnemyState
    {
        Moving
    }

    private void Start()
    {
        currentState = EnemyState.Moving;
    }

    public void Update()
    {
        if(currentState == EnemyState.Moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, World.player.transform.position, moveSpeed * Time.deltaTime);
        }   
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Destroy(gameObject, 0f);
            World.currentEnemyCount--;
            Debug.Log(World.currentEnemyCount);
        }
    }
}
