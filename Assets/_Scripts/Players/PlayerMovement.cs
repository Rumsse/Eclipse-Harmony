using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float turnSpeed = 360;

    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";
    private Vector3 input;

    

    void FixedUpdate()
    {
        GatherInput();
        Move();
        Look();
    }

    void GatherInput()
    {
        input = new Vector3(Input.GetAxisRaw(horizontalInput), 0, Input.GetAxisRaw(verticalInput));
    }

    void Look()
    {
        if (input != Vector3.zero)
        {
            var isoDirection = input.ToIso().normalized;
            var rot = Quaternion.LookRotation(isoDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        if (input != Vector3.zero)
        {
            Vector3 isoInput = input.ToIso().normalized;
            Vector3 targetPosition = transform.position + isoInput;

            agent.SetDestination(targetPosition);
        }
    }

}
