using System;

[Serializable]
public struct MaskAttributes : IEquatable<MaskAttributes>
{
    public MaskShape Shape;
    public EyeColor EyeColor;
    public MaskPattern Pattern;

    public bool Equals(MaskAttributes other)
    {
        return Shape == other.Shape && EyeColor == other.EyeColor && Pattern == other.Pattern;
    }

    public override bool Equals(object obj)
    {
        return obj is MaskAttributes other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (int)Shape;
            hash = hash * 31 + (int)EyeColor;
            hash = hash * 31 + (int)Pattern;
            return hash;
        }
    }
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
