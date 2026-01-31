using System;

[Serializable]
public class MaskOptionData
{
    public MaskAttributes Mask;
    public MaskFitType FitType;
}

public enum MaskFitType
{
    Best = 0,
    Partial = 1,
    None = 2
}
