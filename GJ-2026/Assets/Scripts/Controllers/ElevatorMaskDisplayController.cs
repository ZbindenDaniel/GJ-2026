using System;
using UnityEngine;

public class ElevatorMaskDisplayController : MonoBehaviour
{
    [SerializeField] private int elevatorIndex = -1;
    [SerializeField] private Transform maskRoot;
    [SerializeField] private bool logDetails = false;

    public int ElevatorIndex => elevatorIndex;

    private void Awake()
    {
        if (maskRoot == null)
        {
            maskRoot = transform;
        }
    }

    public void ApplyMask(MaskAttributes mask)
    {
        if (maskRoot == null)
        {
            return;
        }

        string code = MaskCode.Build(mask);
        if (string.IsNullOrWhiteSpace(code))
        {
            if (logDetails)
            {
                Debug.LogWarning($"ElevatorMaskDisplayController missing mask code on {name}.");
            }
            return;
        }

        bool found = false;
        int childCount = maskRoot.childCount;
        if (logDetails)
        {
            Debug.Log($"ElevatorMaskDisplayController applying '{code}' on {name} with {childCount} children.");
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = maskRoot.GetChild(i);
            if (child == null)
            {
                continue;
            }

            string childName = child.name;
            if (childName.EndsWith("(Clone)", StringComparison.OrdinalIgnoreCase))
            {
                childName = childName.Substring(0, childName.Length - "(Clone)".Length);
            }

            bool isHighlight = childName.IndexOf("highlight", StringComparison.OrdinalIgnoreCase) >= 0;
            bool match = string.Equals(childName, code, StringComparison.OrdinalIgnoreCase);
            bool shouldBeActive = match || isHighlight;
            child.gameObject.SetActive(shouldBeActive);
            if (match)
            {
                found = true;
            }
        }

        if (!found && logDetails)
        {
            Debug.LogWarning($"ElevatorMaskDisplayController could not find '{code}' under {name}.");
        }
    }

    public void ClearMask()
    {
        if (maskRoot == null)
        {
            return;
        }

        for (int i = 0; i < maskRoot.childCount; i++)
        {
            Transform child = maskRoot.GetChild(i);
            if (child == null)
            {
                continue;
            }

            string childName = child.name;
            if (childName.EndsWith("(Clone)", StringComparison.OrdinalIgnoreCase))
            {
                childName = childName.Substring(0, childName.Length - "(Clone)".Length);
            }

            bool isHighlight = childName.IndexOf("highlight", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!isHighlight)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // Mask code mapping lives in MaskCode.
}
