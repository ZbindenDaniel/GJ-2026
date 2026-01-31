using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MaskSelectionController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask maskLayer = ~0;
    [SerializeField] private GameControl gameControl;

    private MaskSelectable hovered;
    private bool hasSelected;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (gameControl == null)
        {
            gameControl = FindFirstObjectByType<GameControl>();
        }
    }

    private void Update()
    {
        UpdateHover();

        if (hasSelected)
        {
            return;
        }

        if (IsSelectPressed() && hovered != null)
        {
            if (hovered.Select())
            {
                hasSelected = true;
                if (gameControl != null)
                {
                    gameControl.OnMaskSelected(hovered.MaskAttributes);
                }
            }
        }
    }

    public void ResetSelection()
    {
        hasSelected = false;
        if (hovered != null)
        {
            hovered.SetHighlighted(false);
            hovered = null;
        }
    }

    private void UpdateHover()
    {
        MaskSelectable newHover = null;

        if (playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, maskLayer, QueryTriggerInteraction.Collide))
            {
                newHover = hit.collider.GetComponentInParent<MaskSelectable>();
            }
        }

        if (newHover != hovered)
        {
            if (hovered != null)
            {
                hovered.SetHighlighted(false);
            }

            hovered = newHover;

            if (hovered != null)
            {
                hovered.SetHighlighted(true);
            }
        }
    }

    private static bool IsSelectPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}
