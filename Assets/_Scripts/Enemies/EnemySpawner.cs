using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("---References---")]
    [SerializeField] private Enemy enemyScript;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private EnemyPool enemyPool;

    [Header("---Spawning---")]
    [SerializeField] private float spawnRadius = 1.5f;



    private void Awake()
    {
        Instance = this;
    }

    public void SpawnEnemyFromWave()
    {
        var wave = waveManager.waves[waveManager.currentWaveIndex];

        foreach (var group in wave.enemyGroups)
        {
            if (group.spawnedCount >= group.enemyCount) continue;
            if (!enemyPool.enemyPools.TryGetValue(group.enemyName, out var pool) || pool.Count == 0) continue;

            Enemy enemy = pool.Dequeue();
            Transform target = enemy.target;
            Vector3 spawnPos = GetSpawnPosition(target);

            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);
            enemy.agent.Warp(spawnPos);

            group.spawnedCount++;
            wave.spawnedCount++;
            break;
        }

    }

    public Vector3 GetSpawnPosition(Transform target)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 targetPos = target.position + new Vector3(offset.x, 0, offset.y);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return target.position;
    }

    public Vector3 GetSpawnPosition()
    {
        return GetSpawnPosition(enemyScript.target); 
    }
}
