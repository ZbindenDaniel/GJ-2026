using System.Collections.Generic;
using UnityEngine;

public enum NpcMood
{
    LookAtPlayer,
    Idle,
    Happy,
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
    [SerializeField] private MaskAttributes maskAttributes;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _headTransform;
    [SerializeField] private Transform _bodyTransform;
    [Header("Reaction Motion Settings")]
    [SerializeField] private float _vibeBounceAngle = 12f;
    [SerializeField] private float _vibeBounceSpeed = 3f;
    [SerializeField] private float _assaultShakeAngle = 6f;
    [SerializeField] private float _assaultShakeSpeed = 18f;
    [SerializeField] private float _assaultMoveSpeed = 1.5f;
    [SerializeField] private float _engageBounceHeight = 0.05f;
    [SerializeField] private float _engageBounceSpeed = 2.5f;
    [SerializeField] private float _nodAngle = 15f;
    [SerializeField] private float _nodSpeed = 2f;
    [SerializeField] private float _headShakeAngle = 15f;
    [SerializeField] private float _headShakeSpeed = 2f;
    [SerializeField] private float _playerAwarenessRange = 6f;
    [SerializeField] private float _viewUpdateInterval = 1.5f;
    [SerializeField] private float _reactionInterval = 2f;
    [SerializeField] private float _randomLookRange = 10f;
    [SerializeField] private int _lookAtPlayerCycle = 20;
    [SerializeField] private int _idleResetCycle = 30;
    [Header("Reaction Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _lookAtPlayerClips;
    [SerializeField] private List<AudioClip> _idleClips;
    [SerializeField] private List<AudioClip> _happyClips;
    [SerializeField] private List<AudioClip> _vibeClips;
    [SerializeField] private List<AudioClip> _assaultClips;
    [SerializeField] private List<AudioClip> _engageClips;
    [SerializeField] private List<AudioClip> _noddingClips;
    [SerializeField] private List<AudioClip> _headShakingClips;

    public NpcMood Mood = NpcMood.Idle;

    private NpcMood currentMood = NpcMood.Idle;
    private Vector3 initialViewDirection;
    private Vector3 initialHeadLocalEulerAngles;
    private Vector3 initialHeadLocalPosition;
    private Vector3 initialBodyLocalPosition;
    private Animation _headAnimation;
    private Animation _bodyAnimation;
    private Rigidbody _rigidbody;
    private bool loggedMissingHead;
    private bool loggedMissingBody;
    private bool loggedMissingHeadAnimation;
    private bool loggedMissingBodyAnimation;
    private bool loggedMissingAudioSource;

    private Vector3 targetPoint;
    private float viewTimer;
    private int viewCycleCount;
    private bool lookingAtPlayer;
    private float nextReactionTime;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogWarning($"{name} is missing a Rigidbody; assault movement will be skipped.");
        }

        initialViewDirection = transform.forward;
        CacheMotionOffsets();
        SetupReactionAnimations();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lookingAtPlayer && player != null)
        {
            if (IsPlayerInAwarenessRange())
            {
                targetPoint = player.position;
            }
            else
            {
                lookingAtPlayer = false;
            }
        }
        // Smoothly rotate towards the target point
        Vector3 direction = (targetPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.fixedDeltaTime * 1f);
        }

        TryMoveAssault();

        UpdateMoodState();
        UpdateViewCycle();
    }

    public void LookAtPlayer()
    {
        LookAtPoint(player.position);
        lookingAtPlayer = true;
    }

    public void LookAtPoint(Vector3 viewFocusPoint)
    {
        // Debug.Log(gameObject.name + " is looking at point " + viewFocusPoint);
        lookingAtPlayer = false;
        targetPoint = viewFocusPoint;
    }

    public void Idle()
    {
        // Idle behavior
        LookAtPoint(transform.position + initialViewDirection);
    }

    private void ApplyReactionState(NpcMood newState)
    {
        if (newState == currentMood)
        {
            return;
        }

        currentMood = newState;
        Debug.Log($"{name} reaction state -> {newState}");

        switch (newState)
        {
            case NpcMood.LookAtPlayer:
                ApplyLookAtPlayerMood();
                break;
            case NpcMood.Idle:
                ApplyIdleMood();
                break;
            case NpcMood.Happy:
                ApplyHappyMood();
                break;
            case NpcMood.Vibe:
                ApplyVibeMood();
                break;
            case NpcMood.Assault:
                ApplyAssaultMood();
                break;
            case NpcMood.Engage:
                ApplyEngageMood();
                break;
            case NpcMood.Nodding:
                ApplyNoddingMood();
                break;
            case NpcMood.HeadShaking:
                ApplyHeadShakingMood();
                break;
        }

        PlayMoodAudio(newState);
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

    private void PlayReactionAnimation(NpcMood state)
    {
        try
        {
            switch (state)
            {
                case NpcMood.Vibe:
                    PlayHeadClip("NpcVibe", _vibeBounceSpeed);
                    StopBodyClip();
                    break;
                case NpcMood.Assault:
                    PlayHeadClip("NpcAssault", _assaultShakeSpeed);
                    StopBodyClip();
                    break;
                case NpcMood.Engage:
                    PlayHeadClip("NpcEngageHead", _engageBounceSpeed);
                    PlayBodyClip("NpcEngageBody", _engageBounceSpeed);
                    break;
                case NpcMood.Nodding:
                    PlayHeadClip("NpcNod", _nodSpeed);
                    StopBodyClip();
                    break;
                case NpcMood.HeadShaking:
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

    private void PlayMoodAudio(NpcMood mood)
    {
        if (_audioSource == null)
        {
            if (!loggedMissingAudioSource)
            {
                loggedMissingAudioSource = true;
                Debug.LogWarning($"{name} has no audio source assigned; mood audio will be skipped.");
            }
            return;
        }

        AudioClip clip = GetMoodClip(mood);
        if (clip == null)
        {
            Debug.LogWarning($"{name} has no audio clips assigned for mood {mood}.");
            return;
        }

        try
        {
            _audioSource.PlayOneShot(clip);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to play audio for mood {mood}: {ex.Message}");
        }
    }

    private AudioClip GetMoodClip(NpcMood mood)
    {
        List<AudioClip> clips = null;

        switch (mood)
        {
            case NpcMood.LookAtPlayer:
                clips = _lookAtPlayerClips;
                break;
            case NpcMood.Idle:
                clips = _idleClips;
                break;
            case NpcMood.Happy:
                clips = _happyClips;
                break;
            case NpcMood.Vibe:
                clips = _vibeClips;
                break;
            case NpcMood.Assault:
                clips = _assaultClips;
                break;
            case NpcMood.Engage:
                clips = _engageClips;
                break;
            case NpcMood.Nodding:
                clips = _noddingClips;
                break;
            case NpcMood.HeadShaking:
                clips = _headShakingClips;
                break;
        }

        return GetRandomClip(clips, mood);
    }

    private AudioClip GetRandomClip(List<AudioClip> clips, NpcMood mood)
    {
        if (clips == null || clips.Count == 0)
        {
            return null;
        }

        try
        {
            int index = Random.Range(0, clips.Count);
            AudioClip clip = clips[index];
            if (clip == null)
            {
                Debug.LogWarning($"{name} has an empty audio slot for mood {mood}.");
            }

            return clip;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to select audio for mood {mood}: {ex.Message}");
            return null;
        }
    }

    public void EvaluateMask()
    {
        GameControl gameControl = FindFirstObjectByType<GameControl>();
        if (gameControl == null)
        {
            Debug.LogWarning($"{name} could not find GameControl for mask evaluation.");
            return;
        }

        MaskAttributes playerMask = gameControl.CurrentPlayerMask;
        int matches = 0;

        if (playerMask.Shape == maskAttributes.Shape)
        {
            matches++;
        }

        bool hasFace = playerMask.EyeState != EyeState.None && playerMask.Mouth != MouthMood.None;
        if (hasFace)
        {
            if (playerMask.EyeState == maskAttributes.EyeState)
            {
                matches++;
            }
            if (playerMask.Mouth == maskAttributes.Mouth)
            {
                matches++;
            }
        }

        if (!hasFace)
        {
            SetMood(matches >= 1 ? NpcMood.Nodding : NpcMood.HeadShaking);
            return;
        }

        if (matches >= 2)
        {
            SetMood(NpcMood.Nodding);
        }
        else if (matches == 1)
        {
            SetMood(NpcMood.Idle);
        }
        else
        {
            SetMood(NpcMood.HeadShaking);
        }

        UpdateMoodState();
    }

    private bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return true;
        }

        return other.GetComponent<PlayerController>() != null;
    }

    public void SetMaskAttributes(MaskAttributes attributes)
    {
        maskAttributes = attributes;
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

    public void SetMood(NpcMood newMood)
    {
        if (Mood == newMood)
        {
            return;
        }

        Mood = newMood;
        Debug.Log($"{name} mood set to {Mood}.");
    }

    private void UpdateMoodState()
    {
        if (Mood == currentMood)
        {
            return;
        }

        try
        {
            ApplyReactionState(Mood);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to apply mood {Mood}: {ex.Message}");
        }
    }

    private void UpdateViewCycle()
    {
        if (!ShouldUpdateViewCycle())
        {
            return;
        }

        viewTimer += Time.fixedDeltaTime;
        if (viewTimer < _viewUpdateInterval)
        {
            return;
        }

        viewTimer = 0f;
        viewCycleCount++;

        if (viewCycleCount == _lookAtPlayerCycle)
        {
            TryLookAtPlayer("view cycle look at player");
            return;
        }

        if (viewCycleCount == _idleResetCycle)
        {
            Idle();
            return;
        }

        if (viewCycleCount < _lookAtPlayerCycle)
        {
            LookAtPoint(new Vector3(Random.Range(-_randomLookRange, _randomLookRange), 0f, Random.Range(-_randomLookRange, _randomLookRange)));
        }
    }

    private bool ShouldUpdateViewCycle()
    {
        return currentMood == NpcMood.Idle
            || currentMood == NpcMood.Happy
            || currentMood == NpcMood.Vibe
            || currentMood == NpcMood.Nodding
            || currentMood == NpcMood.HeadShaking;
    }

    private void ApplyLookAtPlayerMood()
    {
        TryLookAtPlayer("look-at-player mood");
        PlayReactionAnimation(NpcMood.LookAtPlayer);
        TriggerAnimatorState(NpcMood.LookAtPlayer);
    }

    private void ApplyIdleMood()
    {
        Idle();
        PlayReactionAnimation(NpcMood.Idle);
        TriggerAnimatorState(NpcMood.Idle);
    }

    private void ApplyHappyMood()
    {
        Idle();
        PlayReactionAnimation(NpcMood.Happy);
        TriggerAnimatorState(NpcMood.Happy);
    }

    private void ApplyVibeMood()
    {
        Idle();
        PlayReactionAnimation(NpcMood.Vibe);
        TriggerAnimatorState(NpcMood.Vibe);
    }

    private void ApplyAssaultMood()
    {
        TryLookAtPlayer("assault mood");
        PlayReactionAnimation(NpcMood.Assault);
        TriggerAnimatorState(NpcMood.Assault);
    }

    private void ApplyEngageMood()
    {
        TryLookAtPlayer("engage mood");
        PlayReactionAnimation(NpcMood.Engage);
        TriggerAnimatorState(NpcMood.Engage);
    }

    private void ApplyNoddingMood()
    {
        Idle();
        PlayReactionAnimation(NpcMood.Nodding);
        TriggerAnimatorState(NpcMood.Nodding);
    }

    private void ApplyHeadShakingMood()
    {
        Idle();
        PlayReactionAnimation(NpcMood.HeadShaking);
        TriggerAnimatorState(NpcMood.HeadShaking);
    }

    private void TryLookAtPlayer(string context)
    {
        if (player != null)
        {
            if (IsPlayerInAwarenessRange())
            {
                LookAtPlayer();
                return;
            }

            // Debug.Log($"{name} skipped looking at player for {context} because they are out of awareness range.");
            return;
        }

        Debug.LogWarning($"{name} has no player assigned for {context}.");
    }

    private bool IsPlayerInAwarenessRange()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 offset = player.position - transform.position;
        return offset.sqrMagnitude <= _playerAwarenessRange * _playerAwarenessRange;
    }

    private void TriggerAnimatorState(NpcMood mood)
    {
        if (_animator == null)
        {
            Debug.LogWarning($"{name} has no animator assigned; skipping reaction animation.");
            return;
        }

        try
        {
            _animator.SetTrigger(mood.ToString());
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to play reaction animation for {mood}: {ex.Message}");
        }
    }

    private void TryMoveAssault()
    {
        if (currentMood != NpcMood.Assault)
        {
            return;
        }

        if (_rigidbody == null || player == null)
        {
            return;
        }

        try
        {
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 moveDirection = targetPosition - transform.position;
            if (moveDirection.sqrMagnitude <= 0.01f)
            {
                return;
            }

            Vector3 step = moveDirection.normalized * _assaultMoveSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(transform.position + step);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{name} failed to move during assault: {ex.Message}");
        }
    }
}
