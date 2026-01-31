using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _floorSounds = new List<AudioClip>();
    [SerializeField] private float _fadeDuration = 1f;

    private float _defaultVolume = 1f;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_audioSource == null)
        {
            Debug.LogWarning("MusicManager is missing an AudioSource reference.", this);
            return;
        }

        _defaultVolume = _audioSource.volume;
    }

    public void PlayFloorSound(int floorIndex)
    {
        try
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("MusicManager cannot play sound because no AudioSource is assigned.", this);
                return;
            }

            if (_floorSounds == null || _floorSounds.Count == 0)
            {
                Debug.LogWarning("MusicManager has no floor sounds configured.", this);
                return;
            }

            if (floorIndex < 0 || floorIndex >= _floorSounds.Count)
            {
                Debug.LogWarning($"MusicManager received out-of-range floor index {floorIndex}.", this);
                return;
            }

            Debug.Log($"MusicManager playing floor sound for floor index {floorIndex}.");
            AudioClip clip = _floorSounds[floorIndex];
            if (clip == null)
            {
                Debug.LogWarning($"MusicManager floor sound at index {floorIndex} is null.", this);
                return;
            }

            _audioSource.PlayOneShot(clip);
        }
        catch (Exception exception)
        {
            Debug.LogError($"MusicManager failed to play floor sound at index {floorIndex}: {exception}", this);
        }
    }

    public void FadeOut()
    {
        try
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("MusicManager cannot fade out because no AudioSource is assigned.", this);
                return;
            }

            StartFade(0f);
        }
        catch (Exception exception)
        {
            Debug.LogError($"MusicManager failed to fade out audio: {exception}", this);
        }
    }

    public void FadeIn()
    {
        try
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("MusicManager cannot fade in because no AudioSource is assigned.", this);
                return;
            }

            if (!_audioSource.isPlaying && _audioSource.clip != null)
            {
                _audioSource.Play();
            }

            StartFade(_defaultVolume);
        }
        catch (Exception exception)
        {
            Debug.LogError($"MusicManager failed to fade in audio: {exception}", this);
        }
    }

    private void StartFade(float targetVolume)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeVolumeRoutine(targetVolume));
    }

    private IEnumerator FadeVolumeRoutine(float targetVolume)
    {
        float startVolume = _audioSource.volume;
        float duration = Mathf.Max(0.01f, _fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        _audioSource.volume = targetVolume;
        if (Mathf.Approximately(targetVolume, 0f))
        {
            _audioSource.Stop();
        }

        fadeRoutine = null;
    }
}
