using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement; // 添加场景管理的引用

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private float eventCooldown = 3f; // 事件冷却时间
    
    [SerializeField] private GridVisualizer gridVisualizer;
    [SerializeField] private CameraController cameraController; // 添加引用
    
    private Dictionary<Vector2Int, GridLocation> grid = new Dictionary<Vector2Int, GridLocation>();
    private HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();
    private Vector2Int playerPosition;
    private Vector2Int destinationPosition;
    
    private float fuel;
    private float food;
    private int remainingDays;
    private int initialDistance; // 存储初始距离
    private bool isGameOver = false;
    private int coins = 0; // 统一使用coins作为货币

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // 订阅设置改变事件
        GameSettings.onSettingsChanged.AddListener(OnSettingsChanged);
    }
    
    private void OnDestroy()
    {
        // 取消订阅
        GameSettings.onSettingsChanged.RemoveListener(OnSettingsChanged);
    }

    private void OnSettingsChanged()
    {
        // 更新资源上限
        fuel = Mathf.Min(fuel, GameConstants.INITIAL_FUEL * 1.5f);
        food = Mathf.Min(food, GameConstants.INITIAL_FOOD * 1.5f);
        
        // 重新计算剩余天数
        int newTotalDays = Mathf.CeilToInt(initialDistance * GameConstants.DISTANCE_MULTIPLIER) + GameConstants.EXTRA_DAYS;
        int daysUsed = newTotalDays - remainingDays;
        remainingDays = Mathf.Max(1, newTotalDays - daysUsed);

        // 更新UI
        UIManager.Instance.UpdateResourceUI(remainingDays, fuel, food, coins);
        
        // 更新所有地点的显示
        gridVisualizer.UpdateAllLocations(grid);
        
        Debug.Log("Game state updated based on new settings");
    }
    
    private void Start()
    {
        if (gridVisualizer == null)
        {
            gridVisualizer = FindAnyObjectByType<GridVisualizer>();
        }
        InitializeGame();
    }
    
    private void Update()
    {
        // 任何时候都可以按R键重启
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    public void InitializeGame()
    {
        // 清理现有数据
        grid.Clear();
        visitedPositions.Clear();
        
        // 设置起点
        playerPosition = Vector2Int.zero;
        
        // 使用设置中的距离参数来生成终点
        int minDist = GameConstants.Settings.minDistance;
        int maxDist = GameConstants.Settings.maxDistance;
        
        // 在最小和最大距离之间随机生成终点
        int totalDistance;
        Vector2Int dest;
        do
        {
            // 随机生成x和y坐标
            int destX = Random.Range(-maxDist, maxDist + 1);
            int destY = Random.Range(-maxDist, maxDist + 1);
            dest = new Vector2Int(destX, destY);
            
            // 计算曼哈顿距离
            totalDistance = Mathf.Abs(destX) + Mathf.Abs(destY);
        }
        // 确保距离在指定范围内
        while (totalDistance < minDist || totalDistance > maxDist);
        
        destinationPosition = dest;
        initialDistance = totalDistance;
        
        // 基于距离设置天数
        remainingDays = Mathf.CeilToInt(initialDistance * GameConstants.DISTANCE_MULTIPLIER) + GameConstants.EXTRA_DAYS;
        
        // 初始化资源
        fuel = GameConstants.INITIAL_FUEL;
        food = GameConstants.INITIAL_FOOD;
        
        // 创建起点和终点
        CreateLocation(playerPosition, LocationType.None, TerrainType.Plains);
        CreateLocation(destinationPosition, LocationType.Destination, TerrainType.Plains);
        
        // 调整相机视角以适应地图
        if (cameraController != null)
        {
            cameraController.AdjustCameraSize(playerPosition, destinationPosition);
        }
        
        // 生成起点周围的位置
        GenerateAdjacentLocations(playerPosition, true);
        
        // 确保在初始化时更新玩家位置显示
        gridVisualizer.UpdatePlayerPosition(playerPosition);
        
        // 更新UI
        UIManager.Instance.UpdateResourceUI(remainingDays, fuel, food, coins);
        UIManager.Instance.ShowMessage($"Distance to destination: {initialDistance} steps");
    }
    
    private void CreateLocation(Vector2Int position, LocationType type, TerrainType terrain)
    {
        if (!grid.ContainsKey(position))
        {
            grid[position] = new GridLocation(position, type, terrain);
            gridVisualizer.CreateLocationVisual(grid[position]);
        }
    }
    
    private void GenerateAdjacentLocations(Vector2Int position, bool ensureOneLocation = false)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };
        
        // 计算到目标的方向
        Vector2Int directionToDestination = (destinationPosition - position);
        
        if (ensureOneLocation)
        {
            // 优先选择朝向目标的方向
            List<Vector2Int> preferredDirs = new List<Vector2Int>();
            foreach (Vector2Int dir in directions)
            {
                if ((dir.x * directionToDestination.x + dir.y * directionToDestination.y) > 0)
                {
                    preferredDirs.Add(dir);
                }
            }
            
            Vector2Int forcedDir = preferredDirs.Count > 0 
                ? preferredDirs[Random.Range(0, preferredDirs.Count)] 
                : directions[Random.Range(0, directions.Length)];
                
            Vector2Int newPos = position + forcedDir;
            
            if (!grid.ContainsKey(newPos))
            {
                LocationType locationType = GetRandomLocationType();
                TerrainType terrainType = GetRandomTerrainType();
                CreateLocation(newPos, locationType, terrainType);
            }
        }
        
        // 生成其他位置
        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = position + dir;
            
            if (!grid.ContainsKey(newPos))
            {
                bool isTowardsDestination = 
                    (dir.x * directionToDestination.x + dir.y * directionToDestination.y) > 0;
                
                float generateChance = isTowardsDestination ? 0.8f : 0.4f;
                
                if (Random.value < generateChance)
                {
                    LocationType locationType = GetRandomLocationType();
                    TerrainType terrainType = GetRandomTerrainType();
                    CreateLocation(newPos, locationType, terrainType);
                }
            }
        }
    }
    
    private bool IsValidPosition(Vector2Int pos)
    {
        // 移除边界检查，允许无限扩展
        return true;
    }
    
    public bool TryMove(Vector2Int direction)
    {
        if (isGameOver) return false;

        Vector2Int newPosition = playerPosition + direction;
        
        if (grid.ContainsKey(newPosition))
        {
            float fuelCost = GetFuelCost(grid[newPosition].Terrain);
            float foodCost = GetFoodCost(grid[newPosition].Terrain);
            
            if (fuel >= fuelCost && food >= foodCost)
            {
                // 更新位置和资源
                playerPosition = newPosition;
                fuel -= fuelCost;
                food -= foodCost;
                remainingDays--;
                
                // 所有地点的冷却时间减1
                foreach (var location in grid.Values)
                {
                    location.DecreaseCooldown();
                }
                
                grid[newPosition].IsDiscovered = true;
                
                // 只在第一次访问位置时生成邻近地点
                if (!visitedPositions.Contains(newPosition))
                {
                    visitedPositions.Add(newPosition);
                    GenerateAdjacentLocations(newPosition, true);
                }
                
                // 更新玩家位置显示
                gridVisualizer.UpdatePlayerPosition(playerPosition);
                gridVisualizer.UpdateAllLocations(grid);
                
                // 触发位置事件
                TriggerLocationEvent(newPosition);
                
                // 更新UI
                UIManager.Instance.UpdateResourceUI(remainingDays, fuel, food, coins);
                
                // 检查游戏结束条件
                CheckGameOver();
                
                return true;
            }
        }
        return false;
    }
    
    private float GetFuelCost(TerrainType terrain)
    {
        // 只保留平原的消耗
        return 1f;
    }
    
    private float GetFoodCost(TerrainType terrain)
    {
        // 只保留平原的消耗
        return 1f;
    }
    
    private void TriggerLocationEvent(Vector2Int position)
    {
        GridLocation location = grid[position];
        
        if (location.IsInCooldown())
            return;

        switch (location.Type)
        {
            case LocationType.Settlement:
                food += GameConstants.SETTLEMENT_FOOD_GAIN;
                UIManager.Instance.ShowMessage("Found food at the settlement!");
                break;
            case LocationType.GasStation:
                fuel += GameConstants.GAS_STATION_FUEL_GAIN;
                UIManager.Instance.ShowMessage("Found fuel at the gas station!");
                break;
            case LocationType.GoldMine:
            case LocationType.CoinSpot:
                int coinGain = location.Type == LocationType.GoldMine ? 
                    Mathf.RoundToInt(GameConstants.Settings.goldMineGain) : 
                    GameConstants.COIN_PICKUP_VALUE;
                coins += coinGain;
                UIManager.Instance.ShowMessage($"Found {coinGain} coins!");
                // 金币点收集后变成普通平原
                location.Type = LocationType.None;
                gridVisualizer.UpdateLocationVisual(location);
                break;
            case LocationType.Trap:
                food -= GameConstants.TRAP_FOOD_LOSS;
                remainingDays -= GameConstants.TRAP_TIME_PENALTY;
                UIManager.Instance.ShowMessage("Trap triggered! Lost food and time...");
                break;
        }

        if (location.Type != LocationType.None && location.Type != LocationType.Destination)
        {
            location.StartCooldown();
        }

        UIManager.Instance.UpdateResourceUI(remainingDays, fuel, food, coins);
        gridVisualizer.UpdateLocationVisual(location);
        CheckGameOver();
    }
    
    private void CheckGameOver()
    {
        if (playerPosition == destinationPosition)
        {
            isGameOver = true;
            UIManager.Instance.ShowGameOver(true);
            return;
        }

        if (remainingDays <= 0 || fuel <= 0 || food <= 0)
        {
            isGameOver = true;
            UIManager.Instance.ShowGameOver(false);
        }
    }

    private LocationType GetRandomLocationType()
    {
        float random = Random.value;
        
        // 50% 概率没有特殊地点
        if (random < 0.5f)
            return LocationType.None;
        
        random = Random.value;
        
        // 剩余50%中的分配
        if (random < 0.25f)
            return LocationType.Settlement;
        else if (random < 0.5f)
            return LocationType.GasStation;
        else if (random < 0.7f)
            return LocationType.CoinSpot;  // 添加金币点生成概率
        else if (random < 0.85f)
            return LocationType.GoldMine;
        else
            return LocationType.Trap;
    }

    private TerrainType GetRandomTerrainType()
    {
        // 移除其他地形，只保留平原
        return TerrainType.Plains;
    }

    public bool TryGetLocation(Vector2Int position, out GridLocation location)
    {
        return grid.TryGetValue(position, out location);
    }

    public void RestartGame()
    {
        // 移除游戏结束检查，直接重启
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public int GetTotalCoins()
    {
        return coins;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateResourceUI(remainingDays, fuel, food, coins);
    }
} 