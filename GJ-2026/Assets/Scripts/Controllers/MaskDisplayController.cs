using System;
using UnityEngine;

public class MaskDisplayController : MonoBehaviour
{
    [SerializeField] private Transform maskRoot;
    [SerializeField] private bool destroyNonSelected = false;
    [SerializeField] private bool logDetails = false;

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
                Debug.LogWarning($"MaskDisplayController missing mask code on {name}.");
            }
            return;
        }

        bool found = false;
        int childCount = maskRoot.childCount;
        if (logDetails)
        {
            Debug.Log($"MaskDisplayController applying '{code}' on {name} with {childCount} children.");
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

            if (!shouldBeActive && destroyNonSelected)
            {
                UnityEngine.Object.Destroy(child.gameObject);
                continue;
            }

            child.gameObject.SetActive(shouldBeActive);
            if (match)
            {
                found = true;
            }
        }

        if (!found)
        {
            if (logDetails)
            {
                System.Collections.Generic.List<string> childNames = new System.Collections.Generic.List<string>();
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

                    childNames.Add(childName);
                }

                Debug.LogWarning($"MaskDisplayController could not find '{code}' under {name}. Available: {string.Join(", ", childNames)}");
            }
            else
            {
                Debug.LogWarning($"MaskDisplayController could not find '{code}' under {name}.");
            }
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
}
