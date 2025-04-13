using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    [Header("Basic Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuration;
    public float pierce; //??
}
