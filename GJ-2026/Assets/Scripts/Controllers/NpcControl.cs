using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class NpcControl : MonoBehaviour
{
    public Transform player;
    private Vector3 initialViewDirection;

    private Vector3 targetPoint;

    void Start()
    {
        initialViewDirection = transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Smoothly rotate towards the target point
        Vector3 direction = (targetPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.fixedDeltaTime * 2f);
        }
        
    }

    public void LookAtPlayer()
    {
        LookAtPoint(player.position);
    }

    public void LookAtPoint(Vector3 viewFocusPoint)
    {
        // Debug.Log(gameObject.name + " is looking at point " + viewFocusPoint);
        
        targetPoint = viewFocusPoint;
    }

    public void Idle()
    {
        // Idle behavior
        LookAtPoint(transform.position + initialViewDirection);
    }
}
