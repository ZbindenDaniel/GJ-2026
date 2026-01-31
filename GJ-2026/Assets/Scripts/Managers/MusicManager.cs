using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _floorSounds = new List<AudioClip>();

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_audioSource == null)
        {
            Debug.LogWarning("MusicManager is missing an AudioSource reference.", this);
        }
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
}
