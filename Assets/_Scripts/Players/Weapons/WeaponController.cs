using Unity.VisualScripting;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    [SerializeField] WeaponStats stats;
    protected float currentCooldown;

    protected virtual void Start()
    {
        currentCooldown = stats.cooldownDuration;
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack();
            currentCooldown = stats.cooldownDuration;
        }
    }

    protected abstract void Attack();
    
}
