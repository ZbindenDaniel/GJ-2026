using System;
using UnityEngine;

public class ElevatorMaskDisplayController : MonoBehaviour
{
    [SerializeField] private int elevatorIndex = -1;
    [SerializeField] private MaskDisplayController maskDisplay;

    public int ElevatorIndex => elevatorIndex;

    private void Awake()
    {
        if (maskDisplay == null)
        {
            maskDisplay = GetComponent<MaskDisplayController>();
        }
    }

    public void ApplyMask(MaskAttributes mask)
    {
        if (maskDisplay != null)
        {
            maskDisplay.ApplyMask(mask);
        }
    }

    public void ClearMask()
    {
        if (maskDisplay != null)
        {
            maskDisplay.ClearMask();
        }
    }

    public bool HasActiveMask()
    {
        if (maskDisplay == null)
        {
            return false;
        }

        Transform root = maskDisplay.transform;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child == null)
            {
                continue;
            }

            string name = child.name;
            if (name.EndsWith("(Clone)", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - "(Clone)".Length);
            }

            bool isHighlight = name.IndexOf("highlight", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!isHighlight && child.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }
}
