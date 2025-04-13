using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Stats")]
    public float moveSpeed;
    public float maxHealth;
    public float damage;
    
}
