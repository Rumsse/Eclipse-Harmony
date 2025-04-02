using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    void Start()
    {
        agent.SetDestination(player.position);
        InvokeRepeating("UpdateTarget", 0.5f, 0.5f);
    }


    void UpdateTarget()
    {
        if (player)
            agent.SetDestination(player.position);
    }

}
