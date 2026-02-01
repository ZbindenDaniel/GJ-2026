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
}
