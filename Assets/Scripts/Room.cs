using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Animator animator;
    private Animator doorAnimator;
    private AudioSource doorOpenSource;
    private Transform[] doors;
    private bool[] neighbors = new bool[] { false, false, false, false };
    private bool done = false;
    private bool doorsDone = false;
    public bool cleared = false;
    private bool doorsOpened = false;
    private bool enemiesSpawned = false;
    private bool canCheckCleared = false;
    private int enemyCount = 0;

    [SerializeField] private GameObject[] enemySpawnPoints;

    void Start()
    {
        doors = new Transform[] { transform.GetChild(0).GetChild(0).GetChild(0), transform.GetChild(0).GetChild(1).GetChild(0), transform.GetChild(0).GetChild(2).GetChild(0), transform.GetChild(0).GetChild(3).GetChild(0) };
        animator = GetComponent<Animator>();
        doorAnimator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        doorOpenSource = GetComponent<AudioSource>();

        if(gameObject.name == "StartingRoom")
        {
            cleared = true;
        }
    }

    private void Update()
    {
        if(!doorsOpened & cleared && doorsDone && World.readyToPlay)
        {
            doorsOpened = true;
            doorAnimator.Play("DoorsOpen", -1, 0f);
            doorOpenSource.Play();
        }

        if(World.currentEnemyCount == 0 && World.currentRoom == this && canCheckCleared)
        {
            cleared = true;
        }

        if(!done)
        {
            if (gameObject.tag != "StartingRoom")
            {
                if (World.waitForGeneration)
                {
                    animator.Play("Hide", -1, 0f);
                    done = World.canHideRooms;
                    doorsDone = true;
                }
                else
                {
                    animator.Play("Hide", -1, 0f);
                    done = true;
                    doorsDone = true;
                }
                GetNeighbors();
            }
            else
            {
                if (!doorsDone)
                {
                    Debug.Log("Called");
                    GetNeighbors();
                    doorsDone = true;
                }
            }
        }
    }

    private void GetNeighbors()
    {
        for(int i = 0; i < 4; i++)
        {
            Collider2D[] collisions = Physics2D.OverlapCircleAll(doors[i].parent.transform.position, 1f);
            if(collisions.Length > 0)
            {
                foreach(Collider2D collider in collisions)
                {
                    if (collider.gameObject.tag == "Occupied Cell" && collider.gameObject != gameObject) 
                    {
                        neighbors[i] = true;
                    }
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            doors[i].gameObject.SetActive(neighbors[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Room" || collision.gameObject.tag == "StartingRoom")
        {
            Debug.Log("Room Collision :(");
        }

        else if(collision.gameObject.tag == "Player")
        {
            World.currentRoom = this;
            World.mainCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            if (!World.discoveredRooms.Contains(gameObject))
            {
                World.discoveredRooms.Add(gameObject);
            }

            for(int i = 0; i < transform.childCount; i++)
            {
                GameObject c = transform.GetChild(i).gameObject;
                if (c.name == "Map Icon")
                {
                   c.SetActive(true);
                }

                if (c.name == "Shadow")
                {
                    if(c.layer == 6)
                    {
                        animator.Play("Unhide Half", -1, 0f);
                    }
                    else
                    {
                        c.SetActive(false);
                    }
                }
            }

            if(!enemiesSpawned)
            {
                enemiesSpawned = true;
                DoEnemySpawn();
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject c = transform.GetChild(i).gameObject;
                if (c.name == "Shadow")
                {
                    c.SetActive(true);
                    c.layer = LayerMask.NameToLayer("Minimap");
                    animator.Play("Hide Half", -1, 0f);
                }
            }
        }
    }

    private Transform[] GetDoors()
    {
        return doors;
    }

    private void DoEnemySpawn()
    {
        StartCoroutine("WaitForSpawn");
    }

    private IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(World.timeBeforeEnemySpawn);

        foreach (GameObject spawnPoint in enemySpawnPoints)
        {
            SpawnEnemies se = spawnPoint.GetComponent<SpawnEnemies>();
            se.Spawn();
            enemyCount += se.enemiesSpawned;
        }
        World.currentEnemyCount = enemyCount;
        canCheckCleared = true;
    }
}
