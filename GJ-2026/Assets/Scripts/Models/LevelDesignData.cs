using System;
using System.Collections.Generic;

[Serializable]
public class LevelDesignData
{
    public int LevelIndex;
    public int NpcCount;
    public int AttributeCount;
    public List<NpcDesignData> Npcs;
    public List<MaskOptionData> AvailableMasks;
    public List<ElevatorDesignData> Elevators;
    public int PlayerElevatorIndex;
}
