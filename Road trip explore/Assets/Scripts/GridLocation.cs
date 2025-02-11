using UnityEngine;
using System;

public class GridLocation
{
    public Vector2Int Position { get; private set; }
    public LocationType Type { get; set; }
    public TerrainType Terrain { get; private set; }
    public bool IsDiscovered { get; set; }
    public int RemainingCooldown { get; set; }

    public GridLocation(Vector2Int position, LocationType type, TerrainType terrain)
    {
        Position = position;
        Type = type;
        Terrain = terrain;
        IsDiscovered = false;
        RemainingCooldown = 0;
    }

    public bool IsInCooldown()
    {
        return RemainingCooldown > 0;
    }

    public void StartCooldown()
    {
        RemainingCooldown = GameConstants.EVENT_COOLDOWN;
    }

    public void DecreaseCooldown()
    {
        if (RemainingCooldown > 0)
            RemainingCooldown--;
    }
}

public enum TerrainType
{
    Plains,     // 平原
    Mountain,   // 山区
    Desert      // 沙漠
} 