using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyState currentState;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private float searchRadius;
    [SerializeField] private float randomMoveDelay;
    [SerializeField] private float sleepTime = 2f;

    public float health;

    private Vector2 target;
    private Rigidbody2D rb;
    private int moveX = 0;
    private int moveY = 0;
    private bool hasDestroyed = false;
    private bool canRandomMove = true;

    private enum EnemyState
    {
        Init,
        Wander,
        Follow,
        Attack,
        Destroy,
        Celebrate
    }

    private void Start()
    {
        currentState = EnemyState.Init;
        rb = GetComponent<Rigidbody2D>();
        animator.speed += Random.Range(-0.25f, 0.25f);
        target = transform.position;

        StartCoroutine("WakeUp");
    }

    public void Update()
    {
        if (currentState != EnemyState.Init)
        {
            HealthCheck();

            if (currentState != EnemyState.Destroy)
            {
                HandleMovement();
                CheckVision();
            }

            switch (currentState)
            {
                case EnemyState.Wander:
                    WanderAndSearch();
                    break;
                case EnemyState.Follow:
                    Follow();
                    break;
                case EnemyState.Destroy:
                    DestroySelf();
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if(target != (Vector2) transform.position)
        {
            Vector2 direction = target - (Vector2)transform.position;
            rb.AddRelativeForce(direction.normalized * moveSpeed, ForceMode2D.Force);
        }
        
    }

    private IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(sleepTime);
        currentState = EnemyState.Wander;
    }

    private void Follow()
    {
        target = World.player.transform.position;
    }

    private void CheckVision()
    {
        if (Vector2.Distance(World.player.transform.position, transform.position) < searchRadius)
        {
            currentState = EnemyState.Follow;
        }
        else
        {
            currentState = EnemyState.Wander;
        }
    }

    private void WanderAndSearch()
    {
        if(canRandomMove)
        {
            canRandomMove = false;
            StartCoroutine("WaitForRandomMove");
        }
    }

    private IEnumerator WaitForRandomMove()
    {
        target = (Vector2) World.currentRoom.transform.position + (Random.insideUnitCircle * 7);
        
        yield return new WaitForSeconds(randomMoveDelay);
        randomMoveDelay = Random.Range(randomMoveDelay - 0.5f, randomMoveDelay + 0.5f);
        canRandomMove = true;
    }

    private void HandleMovement()
    {
        if(target != (Vector2) transform.position)
        {
            if(Vector2.Distance((Vector2)transform.position, target) > 0.5f)
            {
                if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
                {
                    if (rb.velocity.x > 0)
                    {
                        moveX = 1;
                        moveY = 0;
                    }

                    if (rb.velocity.x < 0)
                    {
                        moveX = -1;
                        moveY = 0;
                    }
                }
                else if (Mathf.Abs(rb.velocity.y) > Mathf.Abs(rb.velocity.x))
                {
                    if (rb.velocity.y > 0)
                    {
                        moveX = 0;
                        moveY = 1;
                    }

                    if (rb.velocity.y < 0)
                    {
                        moveX = 0;
                        moveY = -1;
                    }
                }
            }
            else
            {
                target = transform.position;
            }
        }

        animator.SetBool("IsMoving", !(target == (Vector2)transform.position));
        animator.SetFloat("X", moveX);
        animator.SetFloat("Y", moveY);
    }

    private void DestroySelf()
    {
        if (!hasDestroyed)
        {
            hasDestroyed = true;
            Destroy(gameObject, 0f);
            World.currentEnemyCount--;
        }
        
    }

    private void HealthCheck()
    {
        if(health <= 0)
        {
            currentState = EnemyState.Destroy;
        }
    }
}
