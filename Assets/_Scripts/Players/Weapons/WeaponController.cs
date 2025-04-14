using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] WeaponStats stats;
    [SerializeField] Flute flute;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            flute.Fire();
            Debug.Log("fire");
        }
    }
}
