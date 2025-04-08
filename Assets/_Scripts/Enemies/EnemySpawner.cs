using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }

    [System.Serializable]
    public class  EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;
    }

    public List<Wave> waves;
    public int currentWaveCount;

    [Header("Spawner Attributes")]
    float spawnTimer;
    public float waveInterval;

    [Header("----Prototype 1----")]
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





    void Startt()
    {
        player = GameObject.FindWithTag("Player").transform;
        CalculateWaveQuota();
    }

    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer = Time.deltaTime;

        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);

        if(currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }


    void CalculateWaveQuota()
    {
        int currentWaveQuoata = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuoata += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuoata;
        Debug.LogWarning(currentWaveQuoata);
    }


    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    Vector2 spawnPosition = new Vector2(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y + Random.Range(-10f, 10f));
                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                }
            }
        }
    }

}

