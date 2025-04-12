using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyScriptableObject stats;
    public NavMeshAgent agent;
    public Transform target;
    public string targetTag;


    void Start()
    {
        //set health etc

        target = GameObject.FindWithTag(targetTag).transform;
        agent.SetDestination(target.position);
        InvokeRepeating("UpdateTarget", 0.5f, 0.5f);
    }

    void UpdateTarget()
    {
        if (target)
            agent.SetDestination(target.position);
    }

}
