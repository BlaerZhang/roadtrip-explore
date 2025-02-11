using UnityEngine;
using System.Collections.Generic;

public static class GameConstants
{
    private static GameSettings settings;
    
    public static GameSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = Resources.Load<GameSettings>("GameSettings");
                if (settings == null)
                {
                    Debug.LogError("GameSettings not found in Resources folder!");
                }
            }
            return settings;
        }
    }

    // 所有常量改为属性
    public static float DISTANCE_MULTIPLIER => Settings.distanceMultiplier;
    public static int EXTRA_DAYS => Settings.extraDays;
    public static float INITIAL_FUEL => Settings.initialFuel;
    public static float INITIAL_FOOD => Settings.initialFood;
    public static int EVENT_COOLDOWN => Settings.eventCooldown;
    public static float SETTLEMENT_FOOD_GAIN => Settings.settlementFoodGain;
    public static float GAS_STATION_FUEL_GAIN => Settings.gasStationFuelGain;
    public static float TRAP_FOOD_LOSS => Settings.trapFoodLoss;
    public static int TRAP_TIME_PENALTY => Settings.trapTimePenalty;
    public static int COIN_PICKUP_VALUE => Settings.coinPickupValue;
    public static float CELL_SIZE => Settings.cellSize;

    public static class TerrainCost
    {
        public static Dictionary<TerrainType, (float fuel, float food)> Costs =>
            new Dictionary<TerrainType, (float fuel, float food)>
            {
                { TerrainType.Plains, (Settings.plainsFuelCost, Settings.plainsFoodCost) }
            };
    }
} 