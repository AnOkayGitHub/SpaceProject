using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private GameObject spawnPS;

    public int enemiesSpawned = 0;

    public void Spawn()
    {
        GameObject ps = (GameObject)Instantiate(spawnPS);
        ps.transform.position = transform.position;
        Destroy(ps, 1f);

        for(int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = (GameObject) Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
            enemy.transform.position = transform.position;
            Debug.Log(enemy.gameObject.name); 
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
