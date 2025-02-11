using UnityEngine;
using TMPro;

public class LocationEventSystem : MonoBehaviour
{
    public static LocationEventSystem Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerLocationEvent(GridLocation location, ref float fuel, ref float food, ref int remainingDays)
    {
        Debug.Log($"Attempting to trigger event at {location.Position}");

        if (location.IsInCooldown())
        {
            Debug.Log($"Location {location.Position} is in cooldown. Remaining: {location.RemainingCooldown}");
            return;
        }

        bool eventTriggered = false;

        switch (location.Type)
        {
            case LocationType.Settlement:
                food += GameConstants.SETTLEMENT_FOOD_GAIN;
                UIManager.Instance.ShowMessage("Found food at the settlement!");
                eventTriggered = true;
                Debug.Log($"Settlement event triggered at {location.Position}");
                break;
                
            case LocationType.GasStation:
                fuel += GameConstants.GAS_STATION_FUEL_GAIN;
                UIManager.Instance.ShowMessage("Found fuel at the gas station!");
                eventTriggered = true;
                Debug.Log($"Gas station event triggered at {location.Position}");
                break;
                
            case LocationType.Trap:
                food -= GameConstants.TRAP_FOOD_LOSS;
                remainingDays -= GameConstants.TRAP_TIME_PENALTY;
                UIManager.Instance.ShowMessage("Trap triggered! Lost food and time...");
                eventTriggered = true;
                Debug.Log($"Trap event triggered at {location.Position}");
                break;
        }

        if (eventTriggered)
        {
            location.StartCooldown();
            Debug.Log($"Started cooldown at {location.Position}. Cooldown value: {location.RemainingCooldown}");
        }
    }
} 