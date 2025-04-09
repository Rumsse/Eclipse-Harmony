using System.Collections;
using System.Collections.Generic;
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
        public float spawnTime;
        public int spawnedCount; 
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName; 
        public GameObject enemyPrefab; 
        public int enemyCount; 
        public int spawnedCount; 
    }



    [Header("---Wave Settings---")]
    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private bool waveInProgress = false;
    private float spawnTimer = 0f;
    [SerializeField] private float waveTime = 5f;

    [Header("---Pooling---")]
    private Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();
    [SerializeField] private int poolSize = 10;           //POOL SIZE

    [Header("---Spawning---")]
    public Transform player;
    [SerializeField] private float spawnRadius = 1.5f;


    [SerializeField] private Enemy enemyScript;
    public static EnemySpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Tworzymy pule dla ka¿dego unikalnego prefab-u
        foreach (var wave in waves)
        {
            foreach (var group in wave.enemyGroups)
            {
                string key = group.enemyName;

                if (!enemyPools.ContainsKey(key))
                {
                    Queue<GameObject> newPool = new Queue<GameObject>();

                    for (int i = 0; i < poolSize; i++)
                    {
                        GameObject enemy = Instantiate(group.enemyPrefab);
                        enemy.name = key; // nazwa prefab-u (bez "(Clone)")
                        enemy.SetActive(false);
                        newPool.Enqueue(enemy);
                    }

                    enemyPools[key] = newPool;
                }
            }
        }

        StartCoroutine(BeginNextWave());
    }

    private void Update()
    {
        if (waveInProgress)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= waves[currentWaveIndex].spawnTime)
            {
                spawnTimer = 0f;
                SpawnEnemyFromWave();
            }
        }
        else if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(BeginNextWave());
        }
    }

    IEnumerator BeginNextWave()
    {
        waveInProgress = true;
        yield return new WaitForSeconds(waveTime);

        CalculateWaveQuota(waves[currentWaveIndex]);
        spawnTimer = 0f;

        Debug.Log($"Wave {waves[currentWaveIndex].waveName} started!");
    }

    void CalculateWaveQuota(Wave wave)
    {
        wave.waveQuota = 0;
        wave.spawnedCount = 0;

        foreach (var group in wave.enemyGroups)
        {
            group.spawnedCount = 0;
            wave.waveQuota += group.enemyCount;
        }
    }

    public void SpawnEnemyFromWave()
    {
        Wave currentWave = waves[currentWaveIndex];

        if (currentWave.spawnedCount >= currentWave.waveQuota)
        {
            waveInProgress = false;
            currentWaveIndex++;
            return;
        }

        foreach (var group in currentWave.enemyGroups)
        {
            if (group.spawnedCount < group.enemyCount &&
                enemyPools.ContainsKey(group.enemyName) &&
                enemyPools[group.enemyName].Count > 0)
            {
                GameObject enemy = enemyPools[group.enemyName].Dequeue();
                Vector3 spawnPosition = GetSpawnPosition();

                enemy.SetActive(true);
                enemy.transform.position = spawnPosition;

                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.Warp(spawnPosition);
                }

                group.spawnedCount++;
                currentWave.spawnedCount++;
                break; // tylko jeden wróg na raz
            }
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        string key = enemy.name.Trim(); // nazwa prefab-u (bez "(Clone)")

        if (enemyPools.ContainsKey(key))
        {
            enemyPools[key].Enqueue(enemy);
        }
        else
        {
            Debug.LogWarning($"No pool found for: {key}");
        }
    }

    private Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
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
