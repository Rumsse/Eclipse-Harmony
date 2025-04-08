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
    [SerializeField] private float spawnRadius = 1.5f; //dystans od gracza

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Vector3 safeSpawn = GetSpawnPosition();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }

        InvokeRepeating(nameof(SpawnEnemy), 1f, 1f);
    }

    public void SpawnEnemy()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            Vector3 spawnPosition = GetSpawnPosition(); 

            enemy.SetActive(true);
            enemy.transform.position = spawnPosition;

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(spawnPosition); //warp ustawia agent na pozycji na NavMesh
            }
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < 10; i++) //szuka 10 razy miejsca
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 randomPosition = new Vector3(
                player.position.x + randomCircle.x,
                player.position.y,
                player.position.z + randomCircle.y
            );

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return player.position;
    }

}

