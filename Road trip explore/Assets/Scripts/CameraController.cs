using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private float padding = 2f; // 额外的边距

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void AdjustCameraSize(Vector2Int playerPos, Vector2Int destPos)
    {
        // 计算需要显示的区域范围
        float minX = Mathf.Min(playerPos.x, destPos.x);
        float maxX = Mathf.Max(playerPos.x, destPos.x);
        float minY = Mathf.Min(playerPos.y, destPos.y);
        float maxY = Mathf.Max(playerPos.y, destPos.y);

        // 添加边距
        minX -= padding;
        maxX += padding;
        minY -= padding;
        maxY += padding;

        // 计算区域的宽度和高度
        float width = maxX - minX;
        float height = maxY - minY;

        // 计算相机需要的大小
        float cameraSize = Mathf.Max(height / 2, width / mainCamera.aspect / 2);

        // 设置相机大小和位置
        mainCamera.orthographicSize = cameraSize;
        transform.position = new Vector3(
            (minX + maxX) / 2,
            (minY + maxY) / 2,
            transform.position.z
        );
    }
} 