using System;

public static class MaskCode
{
    public static string Build(MaskAttributes mask)
    {
        string shapeCode = GetShapeCode(mask.Shape);
        string eyeCode = GetEyeCode(mask.EyeState);
        if (string.IsNullOrWhiteSpace(shapeCode))
        {
            return null;
        }

        string mouthCode = GetMouthCode(mask.Mouth);
        if (string.IsNullOrWhiteSpace(eyeCode) || string.IsNullOrWhiteSpace(mouthCode))
        {
            return null;
        }

        return $"M{shapeCode}.{eyeCode}{mouthCode}";
    }

    public static string GetShapeCode(MaskShape shape)
    {
        switch (shape)
        {
            case MaskShape.Round:
                return "R";
            case MaskShape.Square:
                return "S";
            case MaskShape.Triangle:
                return "T";
            default:
                return string.Empty;
        }
    }

    public static string GetEyeCode(EyeState eyeState)
    {
        switch (eyeState)
        {
            case EyeState.Smile:
                return "S";
            case EyeState.Neutral:
                return "N";
            case EyeState.Frown:
                return "F";
            case EyeState.None:
                return "O";
            default:
                return string.Empty;
        }
    }

    public static string GetMouthCode(MouthMood mood)
    {
        switch (mood)
        {
            case MouthMood.Happy:
                return "H";
            case MouthMood.Indifferent:
                return "I";
            case MouthMood.Sad:
                return "S";
            case MouthMood.None:
                return "O";
            default:
                return string.Empty;
        }
    }
}
