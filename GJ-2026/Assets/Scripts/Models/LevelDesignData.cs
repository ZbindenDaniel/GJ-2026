using System;
using System.Collections.Generic;

[Serializable]
public class LevelDesignData
{
    public int LevelIndex;
    public int NpcCount;
    public int AttributeCount;
    public List<NpcDesignData> Npcs;
    public List<ElevatorDesignData> Elevators;
    public int PlayerElevatorIndex;
    public MaskAttributes PlayerMask;
    public List<MaskAttributes> LiftChoices;
}
