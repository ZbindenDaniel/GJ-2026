using System;

[Serializable]
public struct MaskAttributes : IEquatable<MaskAttributes>
{
    public MaskShape Shape;
    public EyeState EyeState;
    public MouthMood Mouth;

    public bool Equals(MaskAttributes other)
    {
        return Shape == other.Shape && EyeState == other.EyeState && Mouth == other.Mouth;
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
            hash = hash * 31 + (int)EyeState;
            hash = hash * 31 + (int)Mouth;
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

public enum EyeState
{
    None = -1,
    Smile = 0,
    Neutral = 1,
    Frown = 2
}

public enum MouthMood
{
    None = -1,
    Happy = 0,
    Indifferent = 1,
    Sad = 2
}
