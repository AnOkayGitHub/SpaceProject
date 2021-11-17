using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 100;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projClickDelay = 1f;
    [SerializeField] private float projDestroyTime = 1f;
    [SerializeField] private float projOffset;
    [SerializeField] private Transform projSpawn;
    [SerializeField] private UpdateUI uiUpdater;
    [SerializeField] private AudioSource projSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource doorPassSource;
    [SerializeField] private AudioSource itemPickupSource;
    [SerializeField] private float footstepTime;
    [SerializeField] private float hurtTime;
    [SerializeField] private Color hurtColor;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 movement;
    private GameObject doorTouching;
    private bool isMoving = false;
    private bool canClick = true;
    private int facingX;
    private int facingY;
    private bool canStep = true;
    private bool canChangeColor = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if(World.player == null)
        {
            World.player = gameObject;
        }
    }

    void Update()
    {
        if (World.readyToPlay)
        {
            GetInput();
            DetectDoors();
            GetFacing();

            if (isMoving && canStep)
            {
                canStep = false;
                footstepSource.pitch = Random.Range(0.5f, 0.8f);
                footstepSource.Play();
                StartCoroutine("WaitForFootsteps");
            }
        }

        if(spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, hurtTime * Time.deltaTime);
            if(canChangeColor)
            {
                canChangeColor = false;
                StartCoroutine("WaitForColor");
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            rb.MovePosition(transform.position + movement * Time.deltaTime * speed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

    }

    private IEnumerator WaitForColor()
    {
        yield return new WaitForSeconds(hurtTime * 2f);
        spriteRenderer.color = Color.white;
        canChangeColor = true;
    }

    private IEnumerator WaitForFootsteps()
    {
        yield return new WaitForSeconds(footstepTime);
        canStep = true;
    }

    private void GetFacing()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 distanceToPlayer = worldPosition - transform.position;

        if(Mathf.Abs(distanceToPlayer.x) > Mathf.Abs(distanceToPlayer.y))
        {
            if (worldPosition.x > transform.position.x)
            {
                facingX = 1;
                facingY = 0;
            }
            else if (worldPosition.x < transform.position.x)
            {
                facingX = -1;
                facingY = 0;
            }

        }
        else
        {
            if (worldPosition.y > transform.position.y)
            {
                facingX = 0;
                facingY = 1;
            }
            else if (worldPosition.y < transform.position.y)
            {
                facingX = 0;
                facingY = -1;
            }
        }

        animator.SetFloat("X", facingX);
        animator.SetFloat("Y", facingY);


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

        if (Input.GetKeyDown(KeyCode.E) && World.currentRoom.cleared)
        {
            if (doorTouching != null && doorTouching.transform.GetChild(0).gameObject.activeSelf)
            {
                transform.position = (Vector2)doorTouching.transform.position + doorTouching.GetComponent<Door>().offset;
                doorPassSource.Play();
            }
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            spriteRenderer.color = hurtColor;
        }

        if(Input.GetMouseButton(0) && canClick)
        {
            canClick = false;
            projSource.pitch = Random.Range(0.9f, 1.2f);
            projSource.Play();
            StartCoroutine("WaitForClick");
        }

        isMoving = !(movement.x == 0 && movement.y == 0);
        animator.SetBool("IsMoving", isMoving);
    }

    private IEnumerator WaitForClick()
    {
        CreateProjectile();
        yield return new WaitForSeconds(projClickDelay);
        canClick = true;
    }

    private void CreateProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab);
        proj.transform.position = projSpawn.position;
       
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle + projOffset, Vector3.forward);
        proj.GetComponent<Projectile>().SetLifetime(projDestroyTime);

        

        if(facingY == 1)
        {
            proj.GetComponent<SpriteRenderer>().sortingOrder = -10;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Item")
        {
            RandomizeItem item = collider.transform.GetChild(0).gameObject.GetComponent<RandomizeItem>();
            string n = item.GetName();
            Sprite s = item.GetSprite();

            if (!World.items.ContainsKey(n))
            {
                itemPickupSource.pitch = Random.Range(0.8f, 1.2f);
                itemPickupSource.Play();

                World.items.Add(item.GetName(), item.GetSprite());
                uiUpdater.UpdateItems(n, s);
                Destroy(collider.gameObject, 0f);
            }
            

        }
        else if (collider.gameObject.tag == "Coin")
        {
            Destroy(collider.gameObject, 1f);
            AudioSource coinPickupSource = collider.GetComponent<AudioSource>();
            collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            collider.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            coinPickupSource.pitch = Random.Range(1.2f, 1.4f);
            coinPickupSource.Play();
            World.coins ++;
            uiUpdater.UpdateCoins();
        }
    }
}


