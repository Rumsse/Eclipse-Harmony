using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    [Header("Basic Stats")]
    public string weaponName;
    public GameObject prefab;
    public float damage;
    public float cooldown; //czas pomiêdzy ponownym u¿yciem/atakiem
    public float speed; //jak szybko leci w kierunku przeciwnika czy coœ
    public float fireRate; //czas miêdzy pociskami jeœli podczas jednego u¿ycia/ataku leci kilka pocisków naraz czyli w milisekundach
    public float range;


}
