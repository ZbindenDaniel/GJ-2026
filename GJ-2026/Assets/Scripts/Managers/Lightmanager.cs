using System;
using UnityEngine;

public class Lightmanager : MonoBehaviour
{
    [SerializeField] private float _positionAmplitude = 0.5f;
    [SerializeField] private float _verticalAmplitude = 0.3f;
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private Vector2 _intensityRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private Vector2 _rangeRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private float _phaseOffset = 0.35f;
    [SerializeField] private bool _includeInactiveLights = true;

    private Light[] _lights;
    private Vector3[] _baseLocalPositions;
    private float[] _baseIntensities;
    private float[] _baseRanges;
    private float[] _phaseOffsets;

    private void Awake()
    {
        CacheLights();
    }

    private void Update()
    {
        if (!ValidateCaches())
        {
            return;
        }

        float time = Time.time * _movementSpeed;
        for (int i = 0; i < _lights.Length; i++)
        {
            try
            {
                Light lightInstance = _lights[i];
                if (lightInstance == null)
                {
                    continue;
                }

                float phase = time + _phaseOffsets[i];
                Vector3 offset = new Vector3(
                    Mathf.Sin(phase) * _positionAmplitude,
                    Mathf.Cos(phase) * _verticalAmplitude,
                    Mathf.Sin(phase * 0.7f) * _positionAmplitude
                );

                lightInstance.transform.localPosition = _baseLocalPositions[i] + offset;

                float intensityT = (Mathf.Sin(phase) + 1f) * 0.5f;
                float rangeT = (Mathf.Cos(phase) + 1f) * 0.5f;

                lightInstance.intensity = _baseIntensities[i]
                    * Mathf.Lerp(_intensityRange.x, _intensityRange.y, intensityT);
                lightInstance.range = _baseRanges[i]
                    * Mathf.Lerp(_rangeRange.x, _rangeRange.y, rangeT);
            }
            catch (Exception exception)
            {
                string lightName = _lights[i] != null ? _lights[i].name : "Missing Light";
                Debug.LogError($"Lightmanager failed to update light at index {i} ({lightName}): {exception}", this);
            }
        }
    }

    private void CacheLights()
    {
        _lights = GetComponentsInChildren<Light>(_includeInactiveLights);
        if (_lights == null || _lights.Length == 0)
        {
            Debug.LogWarning("Lightmanager found no lights to animate. Disabling component.", this);
            enabled = false;
            return;
        }

        int lightCount = _lights.Length;
        _baseLocalPositions = new Vector3[lightCount];
        _baseIntensities = new float[lightCount];
        _baseRanges = new float[lightCount];
        _phaseOffsets = new float[lightCount];

        for (int i = 0; i < lightCount; i++)
        {
            Light lightInstance = _lights[i];
            if (lightInstance == null)
            {
                _baseLocalPositions[i] = Vector3.zero;
                _baseIntensities[i] = 0f;
                _baseRanges[i] = 0f;
                _phaseOffsets[i] = i * _phaseOffset;
                continue;
            }

            _baseLocalPositions[i] = lightInstance.transform.localPosition;
            _baseIntensities[i] = lightInstance.intensity;
            _baseRanges[i] = lightInstance.range;
            _phaseOffsets[i] = i * _phaseOffset;
        }
    }

    private bool ValidateCaches()
    {
        if (_lights == null || _lights.Length == 0)
        {
            return false;
        }

        bool lengthsMatch = _baseLocalPositions != null
            && _baseIntensities != null
            && _baseRanges != null
            && _phaseOffsets != null
            && _baseLocalPositions.Length == _lights.Length
            && _baseIntensities.Length == _lights.Length
            && _baseRanges.Length == _lights.Length
            && _phaseOffsets.Length == _lights.Length;

        if (!lengthsMatch)
        {
            Debug.LogWarning("Lightmanager cache mismatch detected. Disabling component to avoid errors.", this);
            enabled = false;
            return false;
        }

        return true;
    }
}
