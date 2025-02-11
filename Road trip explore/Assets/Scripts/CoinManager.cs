using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    private List<Vector2Int> coinSpots = new List<Vector2Int>();
    private int collectedCoins = 0;
    
    private void Start()
    {
        SpawnInitialCoinSpots();
    }
    
    private void SpawnInitialCoinSpots()
    {
        // 随机生成3-5个金币点
        int coinCount = Random.Range(3, 6);
        for (int i = 0; i < coinCount; i++)
        {
            SpawnCoinSpot();
        }
    }
    
    private void SpawnCoinSpot()
    {
        Vector2Int newPosition;
        do
        {
            newPosition = new Vector2Int(
                Random.Range(-10, 11), // -10到10的范围
                Random.Range(-10, 11)
            );
        } while (coinSpots.Contains(newPosition));
        
        coinSpots.Add(newPosition);
        
        GridLocation coinLocation = new GridLocation(newPosition, LocationType.CoinSpot, TerrainType.Plains);
        GridVisualizer.Instance.CreateLocationVisual(coinLocation);
    }
    
    public void CollectCoin(Vector2Int position)
    {
        if (coinSpots.Contains(position))
        {
            collectedCoins += GameConstants.COIN_PICKUP_VALUE;
            coinSpots.Remove(position);
            
            // 金币点变成普通平原
            GridLocation plainLocation = new GridLocation(position, LocationType.None, TerrainType.Plains);
            GridVisualizer.Instance.UpdateLocationVisual(plainLocation);
        }
    }
    
    public void ClaimCoins()
    {
        if (collectedCoins > 0)
        {
            GameManager.Instance.AddCoins(collectedCoins);
            collectedCoins = 0;
        }
    }
} 