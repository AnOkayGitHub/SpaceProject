using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private float lifetime;
    private float currentTime = 0;
    private bool destroy = false;
    private bool hasDestroyed = false;

    [SerializeField] private float speed;
    [SerializeField] private GameObject destroyPS;
    [SerializeField] private GameObject propDestroyPS;
    [SerializeField] private Transform destroySpawn;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float damage;
    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play("BoltFade", -1, 0f);
    }

    private void Update()
    {
        if(currentTime < lifetime)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            if(!destroy)
            {
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject, 0.5f);
        if(!hasDestroyed)
        {
            hasDestroyed = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            audioSource.pitch = Random.Range(0.8f, 1.4f);
            audioSource.Play();
            GameObject ps = (GameObject)Instantiate(destroyPS);
            ps.transform.position = destroySpawn.position;
            Destroy(ps, 1f);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "ProjDestroy")
        {
            if (!hasDestroyed)
            {
                DestroySelf();
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!hasDestroyed)
        {
            DestroySelf();

            if (collision.gameObject.layer == LayerMask.NameToLayer("Prop"))
            {
                Destroy(collision.gameObject, 1f);
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                GameObject ps = (GameObject)Instantiate(propDestroyPS);
                ps.transform.position = collision.transform.position;
                ParticleSystem.MainModule settings = ps.GetComponent<ParticleSystem>().main;
                settings.startColor = new ParticleSystem.MinMaxGradient(collision.gameObject.GetComponent<SpriteRenderer>().color);
                collision.gameObject.GetComponent<Prop>().SpawnLoot();
                collision.gameObject.GetComponent<AudioSource>().pitch = Random.Range(1f, 1.2f);
                collision.gameObject.GetComponent<AudioSource>().Play();
                Destroy(ps, 1f);

            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.health -= damage;
            }
        }
        
    }

    public void SetLifetime(float l)
    {
        lifetime = l;
    }
}
