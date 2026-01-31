using UnityEngine;

public enum NpcReactionState
{
    Idle,
    Happy,
    Aggressive
}

[RequireComponent(typeof(Rigidbody))]
public class NpcControl : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Animator _animator;

    private NpcReactionState currentReactionState = NpcReactionState.Idle;
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

    public void ApplyReactionState(NpcReactionState newState)
    {
        if (newState == currentReactionState)
        {
            return;
        }

        currentReactionState = newState;
        Debug.Log($"{name} reaction state -> {newState}");

        switch (newState)
        {
            case NpcReactionState.Idle:
                Idle();
                break;
            case NpcReactionState.Aggressive:
                if (player != null)
                {
                    LookAtPlayer();
                }
                else
                {
                    Debug.LogWarning($"{name} has no player assigned for aggressive reaction.");
                }
                break;
            case NpcReactionState.Happy:
                Debug.Log($"{name} is happy.");
                break;
        }

        if (_animator == null)
        {
            Debug.LogWarning($"{name} has no animator assigned; skipping reaction animation.");
            return;
        }

        try
        {
            _animator.SetTrigger(newState.ToString());
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to play reaction animation for {newState}: {ex.Message}");
        }
    }
}
