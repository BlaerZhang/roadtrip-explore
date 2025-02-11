using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
public class GameSettings : ScriptableObject
{
    // 添加设置改变事件
    public static UnityEvent onSettingsChanged = new UnityEvent();

    [Header("Game Parameters")]
    [Tooltip("用于计算初始天数：距离 * 这个系数")]
    public float distanceMultiplier = 1.5f;
    
    [Tooltip("额外的天数用于探索和收集资源")]
    public int extraDays = 5;
    
    [Header("Initial Resources")]
    [Tooltip("游戏开始时的燃料数量")]
    public float initialFuel = 30f;
    
    [Tooltip("游戏开始时的食物数量")]
    public float initialFood = 25f;
    
    [Header("Event Settings")]
    [Tooltip("特殊地点使用后的冷却回合数")]
    public int eventCooldown = 3;
    
    [Header("Resource Gains")]
    [Tooltip("到达居住地时获得的食物量")]
    public float settlementFoodGain = 20f;
    
    [Tooltip("到达加油站时获得的燃料量")]
    public float gasStationFuelGain = 25f;
    
    [Tooltip("到达金矿时获得的金币量")]
    public float goldMineGain = 10f;
    
    [Header("Coin Settings")]
    [Tooltip("每个金币点的基础价值")]
    public int coinPickupValue = 10;
    
    [Header("Trap Penalties")]
    [Tooltip("触发陷阱时损失的食物量")]
    public float trapFoodLoss = 8f;
    
    [Tooltip("触发陷阱时损失的天数")]
    public int trapTimePenalty = 1;
    
    [Header("Map Generation")]
    [Tooltip("目标位置与起点的最小距离（曼哈顿距离：只能横向和纵向移动时的最短路径长度）")]
    public int minDistance = 8;
    
    [Tooltip("目标位置与起点的最大距离（曼哈顿距离：|x1-x2| + |y1-y2|，即横向距离加纵向距离）")]
    public int maxDistance = 12;

    [Header("Grid Settings")]
    [Tooltip("网格单元格的大小")]
    public float cellSize = 1f;

    [Header("Terrain Costs")]
    [Tooltip("平原地形的燃料消耗")]
    public float plainsFuelCost = 1f;
    [Tooltip("平原地形的食物消耗")]
    public float plainsFoodCost = 1f;

    private void OnValidate()
    {
        // 当在Inspector中修改值时触发事件
        onSettingsChanged.Invoke();
        Debug.Log("Game settings updated");
    }
} 