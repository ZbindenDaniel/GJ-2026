using UnityEngine;

public static class FloorStylePalette
{
    public static Color GetColor(FloorStyle style)
    {
        switch (style)
        {
            case FloorStyle.Red:
                return new Color(0.9f, 0.25f, 0.2f, 1f);
            case FloorStyle.Green:
                return new Color(0.2f, 0.75f, 0.35f, 1f);
            case FloorStyle.Blue:
                return new Color(0.2f, 0.45f, 0.9f, 1f);
            case FloorStyle.Yellow:
                return new Color(0.95f, 0.85f, 0.2f, 1f);
            case FloorStyle.Purple:
                return new Color(0.65f, 0.35f, 0.85f, 1f);
            case FloorStyle.Cyan:
                return new Color(0.2f, 0.85f, 0.9f, 1f);
            case FloorStyle.Orange:
                return new Color(0.95f, 0.55f, 0.2f, 1f);
            case FloorStyle.Pink:
                return new Color(0.95f, 0.4f, 0.65f, 1f);
            default:
                return Color.white;
        }
    }
}
