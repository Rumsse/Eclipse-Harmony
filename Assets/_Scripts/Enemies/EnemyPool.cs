using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [Header("---References---")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveManager waveManager; 

    [Header("---Pooling---")]
    public Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();
    [SerializeField] private int poolSize = 10;


    private void Start()
    {
        InitializeEnemyPools();
    }

    private void InitializeEnemyPools()
    {
        foreach (var wave in waveManager.waves)
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

    public void ReturnEnemy(GameObject enemy) 
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

}
