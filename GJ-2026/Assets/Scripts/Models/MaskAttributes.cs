using System;

[Serializable]
public struct MaskAttributes
{
    public MaskShape Shape;
    public EyeColor EyeColor;
    public MaskPattern Pattern;
}

public enum MaskShape
{
    Round = 0,
    Square = 1,
    Triangle = 2
}

public enum EyeColor
{
    None = -1,
    Blue = 0,
    Green = 1,
    Red = 2
}

public enum MaskPattern
{
    None = -1,
    Stripes = 0,
    Dots = 1,
    ZigZag = 2
}
