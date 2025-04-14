using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flute : MonoBehaviour
{
    [SerializeField] private WeaponStats stats;
    [SerializeField] private Projectile projectile;
    public Transform fireStartpoint;

    public void Fire()
    {
        fireStartpoint = GameObject.FindWithTag("Player1").transform;
        GameObject target = FindClosesEnemy(stats.range); 
        if (target == null) return;

        Vector3 direction = (target.transform.position - fireStartpoint.position).normalized;

        Instantiate(stats.prefab, fireStartpoint.position, Quaternion.identity);  //zamieniæ gameobject na var 
        projectile.rb.linearVelocity = direction * stats.speed * Time.deltaTime;
    } 

    GameObject FindClosesEnemy(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemy"));
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

        Debug.Log("just debug");
        return closest; 

    }

}
