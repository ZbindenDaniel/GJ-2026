using UnityEngine;

public static class MaskColorPalette
{
    public static Color GetColor(MaskColor color)
    {
        switch (color)
        {
            case MaskColor.Red:
                return new Color(0.9f, 0.25f, 0.2f, 1f);
            case MaskColor.Green:
                return new Color(0.2f, 0.75f, 0.35f, 1f);
            case MaskColor.Blue:
                return new Color(0.2f, 0.45f, 0.9f, 1f);
            case MaskColor.Yellow:
                return new Color(0.95f, 0.85f, 0.2f, 1f);
            case MaskColor.Purple:
                return new Color(0.65f, 0.35f, 0.85f, 1f);
            case MaskColor.Cyan:
                return new Color(0.2f, 0.85f, 0.9f, 1f);
            case MaskColor.Orange:
                return new Color(0.95f, 0.55f, 0.2f, 1f);
            case MaskColor.Pink:
                return new Color(0.95f, 0.4f, 0.65f, 1f);
            default:
                return Color.white;
        }
    }

    public static void ApplyToRenderers(Transform root, MaskColor color)
    {
        if (root == null)
        {
            return;
        }

        Color tint = GetColor(color);
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
            {
                continue;
            }

            string name = renderer.name.ToLowerInvariant();
            if (name.Contains("highlight") || name.Contains("outline") || name.Contains("flare"))
            {
                continue;
            }

            renderer.material.color = tint;
        }
    }
}
