using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "Scriptable Objects/EnemyScriptableObject")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Basic Stats")]
    public float moveSpeed;
    public float maxHealth;
    public float damage;
    
}
