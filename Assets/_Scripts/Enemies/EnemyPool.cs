using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [Header("---References---")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveManager waveManager; 

    [Header("---Pooling---")]
    public Dictionary<string, Queue<Enemy>> enemyPools = new Dictionary<string, Queue<Enemy>>();
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

    private Queue<Enemy> CreatePool(GameObject prefab, string name)
    {
        var pool = new Queue<Enemy>();
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(prefab);
            obj.name = name;
            obj.SetActive(false);

            var enemyComponent = obj.GetComponent<Enemy>();
            pool.Enqueue(enemyComponent);
        }
        return pool;
    }

    public void ReturnEnemy(Enemy enemy) 
    {
        enemy.gameObject.SetActive(false);
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
