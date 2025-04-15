using System.Collections.Generic;
using UnityEngine;

public class Flute : MonoBehaviour
{
    [SerializeField] private WeaponStats stats;
    public Transform fireStartpoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Pooling")]
    private Queue<GameObject> projectilePool = new Queue<GameObject>();
    [SerializeField] private int poolSize = 3;



    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(stats.prefab);
            proj.SetActive(false);

            if (proj.TryGetComponent(out Projectile projectile))
            {
                projectile.SetPool(projectilePool);
            }

            projectilePool.Enqueue(proj);
        }
    }


    public void Fire()
    {
        GameObject target = FindClosesEnemy(stats.range);
        if (target == null) return;

        Vector3 direction = (target.transform.position - fireStartpoint.position).normalized;

        GameObject projectile = GetProjectileFromPool();
        projectile.transform.position = fireStartpoint.position;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        
        if (projectile.TryGetComponent(out Projectile p))
        {
            p.Initialize(direction, stats.speed);
        }

        projectile.SetActive(true);
    }

    GameObject FindClosesEnemy(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);
        float closestDistance = Mathf.Infinity;
        GameObject closest = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closest = hit.gameObject;
                closestDistance = dist;
            }
        }

        return closest;
    }

    private GameObject GetProjectileFromPool()
    {
        if (projectilePool.Count > 0)
        {
            return projectilePool.Dequeue();
        }

        GameObject proj = Instantiate(stats.prefab);
        if (proj.TryGetComponent(out Projectile p))
        {
            p.SetPool(projectilePool);
        }
        return proj;
    }

}
