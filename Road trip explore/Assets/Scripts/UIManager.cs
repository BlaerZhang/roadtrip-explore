using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text messageText;
    
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    
    [SerializeField] private float messageDisplayTime = 3f;
    private float messageTimer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
            {
                messageText.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateResourceUI(int days, float fuel, float food, int coins)
    {
        daysText.text = $"Days: {days}";
        fuelText.text = $"Fuel: {fuel:F1}";
        foodText.text = $"Food: {food:F1}";
        coinsText.text = $"Coins: {coins}";
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        messageTimer = messageDisplayTime;
    }

    public void ShowGameOver(bool isVictory)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = isVictory ? "Victory!\nPress R to restart" : "Game Over\nPress R to restart";
    }
} 