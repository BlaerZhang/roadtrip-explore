using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GridVisualizer : MonoBehaviour
{
    public static GridVisualizer Instance { get; private set; }

    [SerializeField] private GameObject locationPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cooldownTextPrefab;

    private Dictionary<Vector2Int, GameObject> locationObjects = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, TMP_Text> cooldownTexts = new Dictionary<Vector2Int, TMP_Text>();
    private GameObject playerObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 检查必需的预制体
        if (locationPrefab == null)
        {
            Debug.LogError("Location Prefab is not set in GridVisualizer! Please assign it in the inspector.");
            enabled = false; // 禁用组件
            return;
        }
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not set in GridVisualizer! Please assign it in the inspector.");
            enabled = false;
            return;
        }
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        // 将网格坐标转换为世界坐标
        return new Vector3(gridPosition.x, gridPosition.y, 0);
    }

    public void CreateLocationVisual(GridLocation location)
    {
        if (!enabled) return;
        
        if (locationObjects.ContainsKey(location.Position))
            return;

        Vector3 worldPos = GridToWorldPosition(location.Position);
        GameObject locationObj = Instantiate(locationPrefab, worldPos, Quaternion.identity, transform);
        
        SpriteRenderer renderer = locationObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = GetLocationColor(location.Type, location.Terrain);
        }
        
        locationObjects[location.Position] = locationObj;

        // 为可交互地点创建冷却时间显示
        if (cooldownTextPrefab != null && 
            (location.Type == LocationType.Settlement || 
             location.Type == LocationType.GasStation || 
             location.Type == LocationType.Trap))
        {
            // 创建冷却时间文本对象，设置在格子上方
            GameObject cooldownObj = Instantiate(cooldownTextPrefab, locationObj.transform);
            cooldownObj.transform.localPosition = new Vector3(0, 0.3f, -0.1f);

            TMP_Text cooldownText = cooldownObj.GetComponent<TMP_Text>();
            if (cooldownText != null)
            {
                cooldownText.alignment = TextAlignmentOptions.Center;
                cooldownTexts[location.Position] = cooldownText;
                
                // 初始状态设置为隐藏
                cooldownText.gameObject.SetActive(false);
            }
        }
    }

    public void UpdatePlayerPosition(Vector2Int position)
    {
        Vector3 worldPos = GridToWorldPosition(position);
        
        if (playerObject == null)
        {
            playerObject = Instantiate(playerPrefab, worldPos, Quaternion.identity, transform);
        }
        else
        {
            // 确保玩家对象在最上层
            playerObject.transform.position = new Vector3(worldPos.x, worldPos.y, -1);
        }
        
        Debug.Log($"Updated player position to: {position}"); // 调试日志
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(
            gridPos.x * GameConstants.CELL_SIZE, 
            gridPos.y * GameConstants.CELL_SIZE, 
            0
        );
    }

    private Color GetLocationColor(LocationType type, TerrainType terrain)
    {
        switch (type)
        {
            case LocationType.Settlement:
                return new Color(0.3f, 0.6f, 1f);
            case LocationType.GasStation:
                return new Color(1f, 0.5f, 0.5f);
            case LocationType.Trap:
                return new Color(0.5f, 0f, 0f);
            case LocationType.Destination:
                return new Color(1f, 0.3f, 1f);
            case LocationType.GoldMine:
                return new Color(1f, 0.84f, 0f);
            case LocationType.CoinSpot:
                return new Color(1f, 1f, 0f);
            case LocationType.None:
            default:
                return new Color(0.7f, 0.9f, 0.7f); // 平原颜色
        }
    }

    public void UpdateLocationVisual(GridLocation location)
    {
        if (locationObjects.TryGetValue(location.Position, out GameObject locationObj))
        {
            SpriteRenderer renderer = locationObj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                // 更新颜色以反映当前类型
                renderer.color = GetLocationColor(location.Type, location.Terrain);
            }

            if (cooldownTexts.TryGetValue(location.Position, out TMP_Text cooldownText))
            {
                if (location.IsInCooldown())
                {
                    cooldownText.text = location.RemainingCooldown.ToString();
                    cooldownText.gameObject.SetActive(true);
                }
                else
                {
                    cooldownText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateAllLocations(Dictionary<Vector2Int, GridLocation> grid)
    {
        foreach (var location in grid.Values)
        {
            UpdateLocationVisual(location);
        }
    }

    public void ClearGrid()
    {
        // 清理所有位置对象
        foreach (var obj in locationObjects.Values)
        {
            if (obj != null)
                Destroy(obj);
        }
        locationObjects.Clear();
        
        // 清理所有冷却文本
        cooldownTexts.Clear();
        
        // 清理玩家对象
        if (playerObject != null)
        {
            Destroy(playerObject);
            playerObject = null;
        }
    }

    public void RemoveLocationVisual(Vector2Int position)
    {
        if (locationObjects.TryGetValue(position, out GameObject obj))
        {
            Destroy(obj);
            locationObjects.Remove(position);
        }
        
        if (cooldownTexts.ContainsKey(position))
        {
            cooldownTexts.Remove(position);
        }
    }
} 