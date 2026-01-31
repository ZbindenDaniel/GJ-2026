using UnityEngine;

public enum NpcReactionState
{
    LookAtPlayer,

    Idle,
    Happy,
    Aggressive,
    Vibe,
    Assault,
    Engage,
    Nodding,
    HeadShaking
}

[RequireComponent(typeof(Rigidbody))]
public class NpcControl : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _headTransform;
    [SerializeField] private Transform _bodyTransform;
    [Header("Reaction Motion Settings")]
    [SerializeField] private float _vibeBounceAngle = 12f;
    [SerializeField] private float _vibeBounceSpeed = 3f;
    [SerializeField] private float _assaultShakeAngle = 6f;
    [SerializeField] private float _assaultShakeSpeed = 18f;
    [SerializeField] private float _engageBounceHeight = 0.05f;
    [SerializeField] private float _engageBounceSpeed = 2.5f;
    [SerializeField] private float _nodAngle = 15f;
    [SerializeField] private float _nodSpeed = 2f;
    [SerializeField] private float _headShakeAngle = 15f;
    [SerializeField] private float _headShakeSpeed = 2f;

    private NpcReactionState currentReactionState = NpcReactionState.Idle;
    private Vector3 initialViewDirection;
    private Vector3 initialHeadLocalEulerAngles;
    private Vector3 initialHeadLocalPosition;
    private Vector3 initialBodyLocalPosition;
    private Animation _headAnimation;
    private Animation _bodyAnimation;
    private bool loggedMissingHead;
    private bool loggedMissingBody;
    private bool loggedMissingHeadAnimation;
    private bool loggedMissingBodyAnimation;

    private Vector3 targetPoint;

    void Start()
    {
        initialViewDirection = transform.forward;
        CacheMotionOffsets();
        SetupReactionAnimations();
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
            case NpcReactionState.Vibe:
                Idle();
                break;
            case NpcReactionState.Assault:
                if (player != null)
                {
                    LookAtPlayer();
                }
                else
                {
                    Debug.LogWarning($"{name} has no player assigned for assault reaction.");
                }
                break;
            case NpcReactionState.Engage:
                if (player != null)
                {
                    LookAtPlayer();
                }
                else
                {
                    Debug.LogWarning($"{name} has no player assigned for engage reaction.");
                }
                break;
            case NpcReactionState.Nodding:
                Idle();
                break;
            case NpcReactionState.HeadShaking:
                Idle();
                break;
        }

        PlayReactionAnimation(newState);

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

    private void CacheMotionOffsets()
    {
        if (_headTransform != null)
        {
            initialHeadLocalEulerAngles = _headTransform.localEulerAngles;
            initialHeadLocalPosition = _headTransform.localPosition;
        }
        else
        {
            initialHeadLocalEulerAngles = Vector3.zero;
            initialHeadLocalPosition = Vector3.zero;
        }

        if (_bodyTransform != null)
        {
            initialBodyLocalPosition = _bodyTransform.localPosition;
        }
        else
        {
            initialBodyLocalPosition = Vector3.zero;
        }
    }

    private void SetupReactionAnimations()
    {
        if (_headTransform != null)
        {
            _headAnimation = _headTransform.GetComponent<Animation>();
            if (_headAnimation == null)
            {
                _headAnimation = _headTransform.gameObject.AddComponent<Animation>();
            }

            _headAnimation.playAutomatically = false;

            try
            {
                AddHeadClips();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"{name} failed to build head reaction clips: {ex.Message}");
            }
        }
        else if (!loggedMissingHead)
        {
            loggedMissingHead = true;
            Debug.LogWarning($"{name} has no head transform assigned; reaction head motion will be skipped.");
        }

        if (_bodyTransform != null)
        {
            _bodyAnimation = _bodyTransform.GetComponent<Animation>();
            if (_bodyAnimation == null)
            {
                _bodyAnimation = _bodyTransform.gameObject.AddComponent<Animation>();
            }

            _bodyAnimation.playAutomatically = false;

            try
            {
                AddBodyClips();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"{name} failed to build body reaction clips: {ex.Message}");
            }
        }
        else if (!loggedMissingBody)
        {
            loggedMissingBody = true;
            Debug.LogWarning($"{name} has no body transform assigned; body reaction motion will be skipped.");
        }
    }

    private void PlayReactionAnimation(NpcReactionState state)
    {
        try
        {
            switch (state)
            {
                case NpcReactionState.Vibe:
                    PlayHeadClip("NpcVibe", _vibeBounceSpeed);
                    StopBodyClip();
                    break;
                case NpcReactionState.Assault:
                    PlayHeadClip("NpcAssault", _assaultShakeSpeed);
                    StopBodyClip();
                    break;
                case NpcReactionState.Engage:
                    PlayHeadClip("NpcEngageHead", _engageBounceSpeed);
                    PlayBodyClip("NpcEngageBody", _engageBounceSpeed);
                    break;
                case NpcReactionState.Nodding:
                    PlayHeadClip("NpcNod", _nodSpeed);
                    StopBodyClip();
                    break;
                case NpcReactionState.HeadShaking:
                    PlayHeadClip("NpcShake", _headShakeSpeed);
                    StopBodyClip();
                    break;
                default:
                    StopHeadClip();
                    StopBodyClip();
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to play reaction animation for {state}: {ex.Message}");
        }
    }

    private void AddHeadClips()
    {
        if (_headAnimation == null)
        {
            return;
        }

        AddHeadClip("NpcVibe", _vibeBounceAngle, 0f);
        AddHeadClip("NpcAssault", _assaultShakeAngle, _assaultShakeAngle);
        AddHeadClip("NpcEngageHead", _vibeBounceAngle * 0.5f, 0f);
        AddHeadClip("NpcNod", _nodAngle, 0f);
        AddHeadClip("NpcShake", 0f, _headShakeAngle);
    }

    private void AddBodyClips()
    {
        if (_bodyAnimation == null)
        {
            return;
        }

        if (_bodyAnimation.GetClip("NpcEngageBody") != null)
        {
            return;
        }

        AnimationClip clip = new AnimationClip
        {
            legacy = true,
            wrapMode = WrapMode.Loop
        };

        clip.SetCurve("", typeof(Transform), "localPosition.y", BuildOscillationCurve(initialBodyLocalPosition.y, _engageBounceHeight));
        _bodyAnimation.AddClip(clip, "NpcEngageBody");
    }

    private void AddHeadClip(string clipName, float pitchAmplitude, float yawAmplitude)
    {
        if (_headAnimation.GetClip(clipName) != null)
        {
            return;
        }

        AnimationClip clip = new AnimationClip
        {
            legacy = true,
            wrapMode = WrapMode.Loop
        };

        clip.SetCurve("", typeof(Transform), "localEulerAngles.x", BuildOscillationCurve(initialHeadLocalEulerAngles.x, pitchAmplitude));
        clip.SetCurve("", typeof(Transform), "localEulerAngles.y", BuildOscillationCurve(initialHeadLocalEulerAngles.y, yawAmplitude));
        _headAnimation.AddClip(clip, clipName);
    }

    private AnimationCurve BuildOscillationCurve(float baseValue, float amplitude)
    {
        return new AnimationCurve(
            new Keyframe(0f, baseValue),
            new Keyframe(0.25f, baseValue + amplitude),
            new Keyframe(0.5f, baseValue),
            new Keyframe(0.75f, baseValue - amplitude),
            new Keyframe(1f, baseValue)
        );
    }

    private void PlayHeadClip(string clipName, float speed)
    {
        if (_headAnimation == null)
        {
            if (!loggedMissingHeadAnimation)
            {
                loggedMissingHeadAnimation = true;
                Debug.LogWarning($"{name} has no head animation component; cannot play {clipName}.");
            }
            return;
        }

        AnimationState state = _headAnimation[clipName];
        if (state == null)
        {
            Debug.LogWarning($"{name} missing head clip {clipName}.");
            return;
        }

        state.speed = Mathf.Max(0.1f, speed);
        _headAnimation.CrossFade(clipName);
    }

    private void PlayBodyClip(string clipName, float speed)
    {
        if (_bodyAnimation == null)
        {
            if (!loggedMissingBodyAnimation)
            {
                loggedMissingBodyAnimation = true;
                Debug.LogWarning($"{name} has no body animation component; cannot play {clipName}.");
            }
            return;
        }

        AnimationState state = _bodyAnimation[clipName];
        if (state == null)
        {
            Debug.LogWarning($"{name} missing body clip {clipName}.");
            return;
        }

        state.speed = Mathf.Max(0.1f, speed);
        _bodyAnimation.CrossFade(clipName);
    }

    private void StopHeadClip()
    {
        if (_headAnimation != null)
        {
            _headAnimation.Stop();
        }

        if (_headTransform != null)
        {
            _headTransform.localEulerAngles = initialHeadLocalEulerAngles;
            _headTransform.localPosition = initialHeadLocalPosition;
        }
    }

    private void StopBodyClip()
    {
        if (_bodyAnimation != null)
        {
            _bodyAnimation.Stop();
        }

        if (_bodyTransform != null)
        {
            _bodyTransform.localPosition = initialBodyLocalPosition;
        }
    }
}
