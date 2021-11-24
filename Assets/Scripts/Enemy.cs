using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject attackObj;
    [SerializeField] private GameObject attackObj2;
    [SerializeField] private GameObject destroyPrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite[] emotes;
    [SerializeField] private Color hurtColor;
    [SerializeField] private int bossAttackIndex = -1;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float searchRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float randomMoveDelay;
    [SerializeField] private float sleepTime = 2f;
    [SerializeField] private float attackTimer;
    [SerializeField] private float damage;
    [SerializeField] private float attackLifetime;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float hurtTime;
    

    private EnemyState currentState;
    private Vector2 target;
    private SpriteRenderer emote;
    private Rigidbody2D rb;
    private SpriteRenderer gfx;
    private int moveX = 0;
    private int moveY = 0;
    private float currentHealth;
    private bool hasDestroyed = false;
    private bool canRandomMove = true;
    private bool canAttack = true;
    private bool isBoss = false;
    private bool setName = false;
    private bool canChangeColor = true;

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
        rb = GetComponent<Rigidbody2D>();
        gfx = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        emote = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        originalSpeed = moveSpeed;
        
        currentState = EnemyState.Init;
        currentHealth = maxHealth;

        transform.position += (Vector3) Random.insideUnitCircle * UnityEngine.Random.Range(-3.5f, 3.5f);
        searchRadius += Random.Range(-1f, 2f);
        attackRadius += Random.Range(-0.5f, 1.5f);
        animator.speed += Random.Range(-0.25f, 0.25f);

        target = transform.position;
        emote.sprite = emotes[0];

        StartCoroutine("WakeUp");
    }

    private void Update()
    {
        CheckColor();

        if (isBoss)
        {
            UpdateBossUI();
        }

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
                    emote.sprite = emotes[1];
                    break;
                case EnemyState.Follow:
                    Follow();
                    emote.sprite = emotes[2];
                    break;
                case EnemyState.Attack:
                    Attack();
                    emote.sprite = emotes[3];
                    break;
                case EnemyState.Destroy:
                    DestroySelf();
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target != (Vector2)transform.position)
        {
            Vector2 direction = target - (Vector2)transform.position;
            rb.AddRelativeForce(direction.normalized * moveSpeed, ForceMode2D.Force);
        }

    }

    private void Attack()
    {
        if(canAttack)
        {
            canAttack = false;
            Vector3 pos = transform.position;
            Vector3 dir = World.player.transform.position - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (isBoss)
            {
                if (bossAttackIndex == 0)
                {
                    GameObject proj2 = (GameObject)Instantiate(attackObj);
                    proj2.transform.position = transform.position;
                    Projectile p2 = proj2.GetComponent<Projectile>();
                    p2.SetDamage(damage);
                    p2.SetFriendly(false);
                    pos = transform.position;
                    dir = World.player.transform.position - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    proj2.transform.rotation = Quaternion.AngleAxis(angle + 5f, Vector3.forward);
                    proj2.GetComponent<Projectile>().SetLifetime(attackLifetime);

                    GameObject proj3 = (GameObject)Instantiate(attackObj);
                    proj3.transform.position = transform.position;
                    Projectile p3 = proj3.GetComponent<Projectile>();
                    p3.SetDamage(damage);
                    p3.SetFriendly(false);
                    pos = transform.position;
                    dir = World.player.transform.position - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    proj3.transform.rotation = Quaternion.AngleAxis(angle - 5f, Vector3.forward);
                    proj3.GetComponent<Projectile>().SetLifetime(attackLifetime);

                    GameObject proj4 = (GameObject)Instantiate(attackObj2);
                    proj4.transform.position = transform.position;
                    Projectile p4 = proj3.GetComponent<Projectile>();
                    p4.SetDamage(damage + (damage * 1.5f));
                    p4.SetFriendly(false);
                    pos = transform.position;
                    dir = World.player.transform.position - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    proj4.transform.rotation = Quaternion.AngleAxis(angle - 40f, Vector3.forward);
                    proj4.GetComponent<Projectile>().SetLifetime(attackLifetime);

                    GameObject proj5 = (GameObject)Instantiate(attackObj2);
                    proj5.transform.position = transform.position;
                    Projectile p5 = proj3.GetComponent<Projectile>();
                    p5.SetDamage(damage + (damage * 1.5f));
                    p5.SetFriendly(false);
                    pos = transform.position;
                    dir = World.player.transform.position - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    proj5.transform.rotation = Quaternion.AngleAxis(angle + 40f, Vector3.forward);
                    proj5.GetComponent<Projectile>().SetLifetime(attackLifetime);

                    GameObject proj6 = (GameObject)Instantiate(attackObj2);
                    proj6.transform.position = transform.position;
                    Projectile p6 = proj3.GetComponent<Projectile>();
                    p4.SetDamage(damage + (damage * 1.5f));
                    p4.SetFriendly(false);
                    pos = transform.position;
                    dir = World.player.transform.position - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    proj6.transform.rotation = Quaternion.AngleAxis(angle - 45f, Vector3.forward);
                    proj6.GetComponent<Projectile>().SetLifetime(attackLifetime);

                    GameObject proj7 = (GameObject)Instantiate(attackObj2);
                    proj7.transform.position = transform.position;
                    Projectile p7 = proj3.GetComponent<Projectile>();
                    p7.SetDamage(damage + (damage * 1.5f));
                    p7.SetFriendly(false);
                    pos = transform.position;
                    dir = World.player.transform.position - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    proj7.transform.rotation = Quaternion.AngleAxis(angle + 45f, Vector3.forward);
                    proj7.GetComponent<Projectile>().SetLifetime(attackLifetime);

                }
            }

            GameObject proj = (GameObject)Instantiate(attackObj);
            proj.transform.position = transform.position;

            Projectile p = proj.GetComponent<Projectile>();
            p.SetDamage(damage);
            p.SetFriendly(false);

            pos = transform.position;
            dir = World.player.transform.position - pos;
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            proj.GetComponent<Projectile>().SetLifetime(attackLifetime);

            StartCoroutine("WaitForAttack");
        }
    }

    private void UpdateBossUI()
    {
        if(!setName)
        {
            World.bossHUD.SetActive(true);
            setName = true;
            World.bossName.text = World.bossNames[UnityEngine.Random.Range(0, World.bossNames.Length)];
        }
        World.bossHealthbar.fillAmount = (currentHealth / maxHealth);
    }

    private void Follow()
    {
        target = World.player.transform.position;
    }

    private void CheckVision()
    {
        float distance = Vector2.Distance(World.player.transform.position, transform.position);

        if (distance < attackRadius)
        {
            currentState = EnemyState.Attack;
        }
        else if (distance < searchRadius)
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

    private void HandleMovement()
    {

        if(target != (Vector2) transform.position || currentState == EnemyState.Attack)
        {
            if (currentState != EnemyState.Attack)
            {
                if (Vector2.Distance((Vector2)transform.position, target) > 0.5f)
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
            }
            else
            {
                Vector2 p = World.player.transform.position;
                Vector2 diff = p - (Vector2) transform.position;

                if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                {
                    if (p.x < transform.position.x)
                    {
                        moveX = -1;
                        moveY = 0;
                    }
                    else if (p.x > transform.position.x)
                    {
                        moveX = 1;
                        moveY = 0;
                    }
                }
                else
                {
                    if (p.y < transform.position.y)
                    {
                        moveX = 0;
                        moveY = -1;
                    }
                    else if (p.y > transform.position.y)
                    {
                        moveX = 0;
                        moveY = 1;
                    }

                }

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
            if(isBoss)
            {
                World.bossHUD.SetActive(false);
            }

            GameObject destroyPS = (GameObject)Instantiate(destroyPrefab);
            destroyPS.transform.position = transform.position;
            destroyPS.gameObject.GetComponent<AudioSource>().pitch += Random.Range(-0.02f, 0.02f);
            float newScale = Random.Range(transform.localScale.x - 0.5f, transform.localScale.x + 0.5f);
            destroyPS.transform.localScale = new Vector3(newScale, newScale, newScale);

            hasDestroyed = true;
            Destroy(destroyPS, 2f);
            Destroy(gameObject, 0f);
            
            World.currentEnemyCount--;
        }
        
    }

    private void HealthCheck()
    {
        if(currentHealth <= 0)
        {
            currentState = EnemyState.Destroy;
        }
    }

    private void CheckColor()
    {
        if (gfx.color != Color.white)
        {
            gfx.color = Color.Lerp(gfx.color, Color.white, hurtTime * Time.deltaTime);
            if (canChangeColor)
            {
                canChangeColor = false;
                StartCoroutine("WaitForColor");
            }
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void Hurt(float damage)
    {
        Debug.Log("currentHealth = " + currentHealth.ToString());
        currentHealth -= damage;
        gfx.color = hurtColor;
    }

    public void SetBossStatus(bool status)
    {
        isBoss = status;
    }


    private IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(sleepTime);
        currentState = EnemyState.Wander;
    }

    private IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(attackTimer - 0.5f, attackTimer + 0.5f));
        canAttack = true;
    }

    private IEnumerator WaitForRandomMove()
    {
        target = (Vector2)World.currentRoom.transform.position + (UnityEngine.Random.insideUnitCircle * 7);

        yield return new WaitForSeconds(randomMoveDelay);
        randomMoveDelay = UnityEngine.Random.Range(randomMoveDelay - 0.5f, randomMoveDelay + 0.5f);
        canRandomMove = true;
    }

    private IEnumerator WaitForColor()
    {
        yield return new WaitForSeconds(hurtTime * 2f);
        gfx.color = Color.white;
        canChangeColor = true;
    }

    
}
