using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NpcMaskInteractionController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask maskLayer = ~0;

    private MaskSelectable hoveredSelectable;
    private NpcControl hoveredNpc;
    private bool loggedMissingSelectable;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        UpdateHover();

        if (hoveredNpc == null)
        {
            return;
        }

        if (IsSelectPressed())
        {
            TryEvaluateMask(hoveredNpc);
        }
    }

    private void UpdateHover()
    {
        MaskSelectable newSelectable = null;
        NpcControl newNpc = null;

        if (playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, maskLayer, QueryTriggerInteraction.Collide))
            {
                NpcControl npcControl = hit.collider.GetComponentInParent<NpcControl>();
                if (npcControl != null)
                {
                    newSelectable = hit.collider.GetComponentInParent<MaskSelectable>();
                    if (newSelectable == null)
                    {
                        if (!loggedMissingSelectable)
                        {
                            loggedMissingSelectable = true;
                            Debug.LogWarning($"NpcMaskInteractionController: {npcControl.name} has no MaskSelectable on the mask child.");
                        }
                    }
                    else
                    {
                        newNpc = npcControl;
                    }
                }
            }
        }

        if (newSelectable != hoveredSelectable)
        {
            if (hoveredSelectable != null)
            {
                hoveredSelectable.SetHighlighted(false);
            }

            hoveredSelectable = newSelectable;

            if (hoveredSelectable != null)
            {
                hoveredSelectable.SetHighlighted(true);
            }
        }

        hoveredNpc = newNpc;
    }

    private void TryEvaluateMask(NpcControl npcControl)
    {
        if (npcControl == null)
        {
            return;
        }

        try
        {
            npcControl.EvaluateMask();
            Debug.Log($"NpcMaskInteractionController selected mask on {npcControl.name}.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"NpcMaskInteractionController failed to evaluate mask on {npcControl.name}: {ex.Message}");
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
