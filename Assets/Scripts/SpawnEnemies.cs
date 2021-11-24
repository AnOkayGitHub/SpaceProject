using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private GameObject spawnPS;
    [SerializeField] private bool spawnBoss = false;

    public int enemiesSpawned = 0;
    public void Spawn()
    {

        for(int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = (GameObject) Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
            enemy.transform.position = transform.position;

            if(spawnBoss)
            {
                enemy.GetComponent<Enemy>().SetBossStatus(true);
            }

            if(enemy.GetComponent<Rigidbody2D>() != null)
            {
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-80, 80), Random.Range(-80, 80)));
            }
            else
            {
                Debug.Log("What");
            }
            enemiesSpawned += 1;
        }
    }
}
