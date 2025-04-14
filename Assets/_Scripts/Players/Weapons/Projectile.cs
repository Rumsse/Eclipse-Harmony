using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private WeaponStats stats;
    public Rigidbody rb;
    private Vector3 direction;
    private bool hasHit = false;


    void Update()
    {
        transform.position += direction * stats.speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;
            Debug.Log($"{gameObject.name} has taken damage.");
        }
    }
}
