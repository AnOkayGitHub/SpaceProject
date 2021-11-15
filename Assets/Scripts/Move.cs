using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 100;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private Vector3 movement;
    private GameObject doorTouching;
    private bool isMoving = false;
    private int facing = 4;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(World.readyToPlay)
        {
            GetInput();
            DetectDoors();
            GetFacing();
        }
    }

    void FixedUpdate()
    {
        if(isMoving)
        {
            rb.MovePosition(transform.position + movement * Time.deltaTime * speed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        
    }

    private void GetFacing()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 distanceToPlayer = worldPosition - transform.position;

        if(Mathf.Abs(distanceToPlayer.x) > Mathf.Abs(distanceToPlayer.y))
        {
            if (worldPosition.x > transform.position.x)
            {
                animator.SetFloat("X", 1);
                animator.SetFloat("Y", 0);
            }
            else if (worldPosition.x < transform.position.x)
            {
                animator.SetFloat("X", -1);
                animator.SetFloat("Y", 0);
            }

        }
        else
        {
            if (worldPosition.y > transform.position.y)
            {
                animator.SetFloat("Y", 1);
                animator.SetFloat("X", 0);
            }
            else if (worldPosition.y < transform.position.y)
            {
                animator.SetFloat("Y", -1);
                animator.SetFloat("X", 0);
            }
        }

        
    }

    private void DetectDoors()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 0.6f);


        foreach (Collider2D col in found)
        {
            if (col.gameObject.tag == "Door")
            {
                doorTouching = col.gameObject;
                break;
            }
            else
            {
                doorTouching = null;
            }
        }
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                movement.y = 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movement.y = -1;
            }
        }
        else
        {
            movement.y = 0;
        }


        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.D))
            {
                movement.x = 1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movement.x = -1;
            }
        }
        else
        {
            movement.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (doorTouching != null && doorTouching.transform.GetChild(0).gameObject.activeSelf)
            {
                transform.position = (Vector2)doorTouching.transform.position + doorTouching.GetComponent<Door>().offset;
            }
        }

        isMoving = !(movement.x == 0 && movement.y == 0);
        animator.SetBool("IsMoving", isMoving);


    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Item")
        {
            Destroy(collider.gameObject, 0f);
        }
    }
}


