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
    [SerializeField] private float timeBetweenWaves = 5f;

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
        InitializeEnemyPools();
        StartCoroutine(BeginNextWave());
    }

    private void InitializeEnemyPools()
    {
        foreach (var wave in waves)
            foreach (var group in wave.enemyGroups)
            {
                if (enemyPools.ContainsKey(group.enemyName)) continue;

                enemyPools[group.enemyName] = CreatePool(group.enemyPrefab, group.enemyName);
            }
    }

    private Queue<GameObject> CreatePool(GameObject prefab, string name)
    {
        var pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            var enemy = Instantiate(prefab);
            enemy.name = name;
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
        return pool;
    }


    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            yield break;
        }

        var wave = waves[currentWaveIndex];
        PrepareWave(wave);
        Debug.Log($"Wave {wave.waveName} started!");

        yield return StartCoroutine(RunWave(wave));
    }

    IEnumerator RunWave(Wave wave)
    {
        while (wave.spawnedCount < wave.waveQuota)
        {
            SpawnEnemyFromWave();
            yield return new WaitForSeconds(wave.spawnTime);
        }

        currentWaveIndex++;
        StartCoroutine(BeginNextWave());
    }

    void PrepareWave(Wave wave)    // wczeœniej jako CalculateWaveQuota()
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
        var wave = waves[currentWaveIndex];

        foreach (var group in wave.enemyGroups)
        {
            if (group.spawnedCount >= group.enemyCount) continue;
            if (!enemyPools.TryGetValue(group.enemyName, out var pool) || pool.Count == 0) continue;

            var enemy = pool.Dequeue();
            Vector3 spawnPos = GetSpawnPosition();

            enemy.transform.position = spawnPos;
            enemy.SetActive(true);

            enemy.GetComponent<NavMeshAgent>()?.Warp(spawnPos);

            group.spawnedCount++;
            wave.spawnedCount++;
            break;
        }
    }

    public void ReturnEnemy(GameObject enemy)    // to jest do object poolingu, gdy enemy umrze, dlatego teraz jest 0 odwo³añ
    {
        enemy.SetActive(false);
        if (enemyPools.TryGetValue(enemy.name.Trim(), out var pool))
        {
            pool.Enqueue(enemy);
        }
        else
        {
            Debug.LogWarning($"No pool found for: {enemy.name}");
        }
    }

    private Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 targetPos = player.position + new Vector3(offset.x, 0, offset.y);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return player.position;
    }


}
