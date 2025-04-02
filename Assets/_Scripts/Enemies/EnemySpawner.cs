using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    public static EnemySpawner Instance;
    public Transform player;

    [SerializeField] private int poolSize = 20;
    [SerializeField] private float spawnRadius = 3f; // Dystans od gracza

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    public void SpawnEnemy()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.SetActive(true);
            enemy.transform.position = GetSpawnPosition(); // Spawn wokó³ gracza
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        return new Vector3(player.position.x + randomCircle.x, player.position.y, player.position.z + randomCircle.y);
    }
}

