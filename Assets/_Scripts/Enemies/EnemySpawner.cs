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

    public Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 targetPos = enemyScript.target.position + new Vector3(offset.x, 0, offset.y);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return enemyScript.target.position;
    }


}
