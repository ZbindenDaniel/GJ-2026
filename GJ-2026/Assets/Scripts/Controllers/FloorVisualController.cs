using UnityEngine;

public class FloorVisualController : MonoBehaviour
{
    [SerializeField] private Transform rendererRoot;
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private bool includeInactive = true;

    private void Awake()
    {
        if (rendererRoot == null)
        {
            rendererRoot = transform;
        }

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = rendererRoot.GetComponentsInChildren<Renderer>(includeInactive);
        }
    }

    public void ApplyStyle(FloorStyle style)
    {
        Color tint = FloorStylePalette.GetColor(style);
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            return;
        }

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            Renderer renderer = targetRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            renderer.material.color = tint;
        }
    }
}
