using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }
    }

    private void Update()
    {
        // 只在游戏运行时处理输入
        if (gameManager == null) return;

        Vector2Int moveDirection = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = Vector2Int.up;
            Debug.Log("Moving up"); // 调试日志
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = Vector2Int.down;
            Debug.Log("Moving down"); // 调试日志
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = Vector2Int.left;
            Debug.Log("Moving left"); // 调试日志
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = Vector2Int.right;
            Debug.Log("Moving right"); // 调试日志
        }

        // 如果有移动输入，尝试移动
        if (moveDirection != Vector2Int.zero)
        {
            bool moved = gameManager.TryMove(moveDirection);
            Debug.Log($"Move attempt: {moved}"); // 调试日志
        }
    }
} 