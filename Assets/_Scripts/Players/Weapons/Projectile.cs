using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private GameObject target;

    private Queue<GameObject> pool;


    public void Initialize(Vector3 dir, float spd)
    {
        direction = dir;
        speed = spd;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Projectile hit enemy: " + other.name);
            //dodaæ dmg, health etc
            gameObject.SetActive(false);
            pool.Enqueue(gameObject);
        }
    }

    public void SetPool(Queue<GameObject> poolReference)
    {
        pool = poolReference;
    }

    private void OnDisable()
    {
        direction = Vector3.zero;
        speed = 0f;
    }

}
