using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnTime;
        public int spawnedCount;
        public bool isBossWave;
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
    public int currentWaveIndex = 0;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("---References---")]
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] EnemyPool enemyPool;

    

    void Start()
    {
        StartCoroutine(BeginNextWave());
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
        if (wave.isBossWave)
        {
            yield return StartCoroutine(RunBossWave(wave));
        }
        else
        {
            yield return StartCoroutine(RunNormalWave(wave));
        }

        currentWaveIndex++;
        StartCoroutine(BeginNextWave());
    }

    IEnumerator RunNormalWave(Wave wave)
    {
        while (wave.spawnedCount < wave.waveQuota)
        {
            enemySpawner.SpawnEnemyFromWave();
            yield return new WaitForSeconds(wave.spawnTime);
        }
    }

    IEnumerator RunBossWave(Wave wave)
    {
        while (AnyEnemiesAlive())
            yield return null;

        yield return new WaitForSeconds(6f); // chwila przerwy na oddech

        Debug.Log($"[BOSS] Wave {wave.waveName} started!");

        // Zak³adamy, ¿e boss to po prostu 1 prefab w grupie
        var group = wave.enemyGroups[0];
        if (!enemyPool.enemyPools.TryGetValue(group.enemyName, out var pool) || pool.Count == 0)
            yield break;

        var boss = pool.Dequeue();
        boss.transform.position = enemySpawner.GetSpawnPosition();
        boss.gameObject.SetActive(true);

        group.spawnedCount++;
        wave.spawnedCount++;

        // Czekaj a¿ boss padnie (mo¿esz to obs³u¿yæ np. przez event)
        while (boss.gameObject.activeSelf)
            yield return null;

        Debug.Log("[BOSS] Defeated!");
    }

    void PrepareWave(Wave wave)
    {
        wave.waveQuota = 0;
        wave.spawnedCount = 0;

        foreach (var group in wave.enemyGroups)
        {
            group.spawnedCount = 0;
            wave.waveQuota += group.enemyCount;
        }
    }

    private bool AnyEnemiesAlive()
    {
        foreach (var pool in enemyPool.enemyPools.Values)
        {
            foreach (var enemy in pool)
            {
                if (enemy.gameObject.activeSelf)
                    return true;
            }
        }
        return false;
    }


}
