using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource projSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource doorPassSource;
    [SerializeField] private AudioSource itemPickupSource;
    [SerializeField] private Transform projSpawn;
    [SerializeField] private UpdateUI uiUpdater;
    [SerializeField] private SpriteRenderer gfx;
    [SerializeField] private VolumeProfile volumeProfile;
    [SerializeField] private Color hurtColor;
    [SerializeField] private Color boosterColor;
    [SerializeField] private Color armorColor;
    [SerializeField] private Color nvgColor;

    [SerializeField] private float speed = 100;
    [SerializeField] private float projClickDelay = 1f;
    [SerializeField] private float projDestroyTime = 1f;
    [SerializeField] private float projOffset;
    [SerializeField] private float footstepTime;
    [SerializeField] private float hurtTime;
    [SerializeField] private float upgradedDamage;
    [SerializeField] private float invincibleModeTime = 0.5f;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float damage;
    
    private GameObject doorTouching;
    private GameObject interactableTouching;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private ColorAdjustments colorAdjustment;
    private ColorAdjustments caVal;
    private Vector3 movement;
    private float currentHealth;
    private float pegLegSpeed;
    private float boosterClickDelay;
    private int facingX;
    private int facingY;
    private bool canHurt = true;
    private bool isMoving = false;
    private bool canClick = true;
    private bool canStep = true;
    private bool canChangeColor = true;
    private bool[] hasItem = new bool[] { false, false, false, false, false, false, false };
    
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (World.player == null)
        {
            World.player = gameObject;
        }

        if (volumeProfile.TryGet<ColorAdjustments>(out caVal)) 
        { 
            colorAdjustment = caVal; 
        }

        pegLegSpeed = speed * 1.5f;
        boosterClickDelay = projClickDelay / 1.5f;
        World.startLocation = transform.position;
    }

    void Update()
    {
        UpdateItemEffects();
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        if (World.readyToPlay)
        {
            GetInput();
            DetectDoors();
            DetectTransition();
            GetFacing();

            if (isMoving && canStep)
            {
                canStep = false;
                footstepSource.pitch = Random.Range(0.5f, 0.8f);
                footstepSource.Play();
                StartCoroutine("WaitForFootsteps");
            }
        }

        if (spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, hurtTime * Time.deltaTime);
            if (canChangeColor)
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Item")
        {
            RandomizeItem item = collider.transform.GetChild(0).gameObject.GetComponent<RandomizeItem>();
            string n = item.GetName();
            Sprite s = item.GetSprite();
            string d = item.GetDesc();

            if (!World.items.ContainsKey(n))
            {
                itemPickupSource.pitch = Random.Range(0.8f, 1.2f);
                itemPickupSource.Play();

                World.items.Add(item.GetName(), item.GetSprite());
                uiUpdater.UpdateItems(n, s, d);
                Destroy(collider.gameObject, 0f);
            }

            UpdateItemEffects();



        }
        else if (collider.gameObject.tag == "Coin")
        {
            Destroy(collider.gameObject, 1f);
            AudioSource coinPickupSource = collider.GetComponent<AudioSource>();
            collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            collider.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            coinPickupSource.pitch = Random.Range(1.2f, 1.4f);
            coinPickupSource.Play();

            World.coins++;
            uiUpdater.UpdateCoins();
        }
        else if (collider.gameObject.tag == "HP" && currentHealth < maxHealth)
        {
            Destroy(collider.gameObject, 1f);
            AudioSource hpAudioSource = collider.GetComponent<AudioSource>();
            collider.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            collider.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            hpAudioSource.pitch = Random.Range(1.2f, 1.4f);
            hpAudioSource.Play();
            currentHealth += 15f;
            UpdateHealth();
        }
    }

    private void UpdateItemEffects()
    {
        foreach(KeyValuePair<string, Sprite> kvp in World.items)
        {
            switch(kvp.Key)
            {
                case "Vaccine":
                    hasItem[0] = true;
                    break;
                case "NVG":
                    hasItem[1] = true;
                    World.globalLight.intensity = 0.6f;
                    colorAdjustment.active = true;
                    colorAdjustment.colorFilter.value = nvgColor;
                    colorAdjustment.saturation.value = 1;
                    break;
                case "Bionic Peg Leg":
                    hasItem[2] = true;
                    speed = pegLegSpeed;
                    break;
                case "Suit Upgrade":
                    hasItem[3] = true;
                    break;
                case "Alien Tech":
                    hasItem[4] = true;
                    break;
                case "Tesla Coil":
                    hasItem[5] = true;
                    break;
                case "Armor":
                    hasItem[6] = true;
                    uiUpdater.GetPlayerHealthbar().color = armorColor;
                    gfx.color = armorColor;
                    maxHealth = 200;
                    currentHealth = maxHealth;
                    break;
            }
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

    private void DetectTransition()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 0.6f);


        foreach (Collider2D col in found)
        {
            if (col.gameObject.tag == "Transition")
            {
                interactableTouching = col.gameObject;
                break;
            }
            else
            {
                interactableTouching = null;
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
                transform.position = (Vector2)doorTouching.transform.position + doorTouching.GetComponent<Door>().GetOffset();
                doorPassSource.Play();
            }
            else if (interactableTouching != null)
            {
                foreach(GameObject g in GameObject.FindGameObjectsWithTag("Coin"))
                {
                    Destroy(g, 0f);
                }

                foreach (GameObject g in GameObject.FindGameObjectsWithTag("HP"))
                {
                    Destroy(g, 0f);
                }

                World.level ++;
                World.levelManager.Create();
                transform.position = World.startLocation;
                
            }
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

    private void CreateProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab);
        proj.transform.position = projSpawn.position;
        Projectile p = proj.GetComponent<Projectile>();


        if (hasItem[0])
        {
            proj.GetComponent<SpriteRenderer>().color = boosterColor;
            projClickDelay = boosterClickDelay;
        }

        if (hasItem[3])
        {
            p.SetDamage(upgradedDamage);
        }
        else
        {
            p.SetDamage(damage);
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle + projOffset, Vector3.forward);
        proj.GetComponent<Projectile>().SetLifetime(projDestroyTime);

        if (hasItem[5])
        {

            GameObject proj2 = Instantiate(projectilePrefab);
            proj2.transform.position = projSpawn.position;
            proj2.transform.rotation = Quaternion.AngleAxis(angle + projOffset + 15, Vector3.forward);
            proj2.GetComponent<Projectile>().SetLifetime(projDestroyTime);
            Projectile p2 = proj2.GetComponent<Projectile>();


            GameObject proj3 = Instantiate(projectilePrefab);
            proj3.transform.position = projSpawn.position;
            proj3.transform.rotation = Quaternion.AngleAxis(angle + projOffset - 15, Vector3.forward);
            proj3.GetComponent<Projectile>().SetLifetime(projDestroyTime);
            Projectile p3 = proj3.GetComponent<Projectile>();

            if (hasItem[3])
            {
                p.SetDamage(upgradedDamage / 3);
                p2.SetDamage(upgradedDamage / 3);
                p3.SetDamage(upgradedDamage / 3);
            }
            else
            {
                p.SetDamage(damage / 3);
                p2.SetDamage(damage / 3);
                p3.SetDamage(damage / 3);
            }

        }

        if (facingY == 1)
        {
            proj.GetComponent<SpriteRenderer>().sortingOrder = -10;
        }
    }

    public void Hurt(float damage)
    {
        if (canHurt)
        {
            canHurt = false;
            currentHealth -= damage;
            UpdateHealth();

            spriteRenderer.color = hurtColor;
            StartCoroutine("WaitForInvincibility");
        }
    }

    public void UpdateHealth()
    {
        uiUpdater.UpdateHealthbar(currentHealth, maxHealth);
    }

    public bool[] GetItems()
    {
        return hasItem;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetCurrentHealth(float val)
    {
        currentHealth = val;
    }

    private IEnumerator WaitForInvincibility()
    {
        yield return new WaitForSeconds(invincibleModeTime);
        canHurt = true;
    }

    private IEnumerator WaitForClick()
    {
        CreateProjectile();
        yield return new WaitForSeconds(projClickDelay);
        canClick = true;
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
    
}