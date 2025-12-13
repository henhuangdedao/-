using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMouse : MonoBehaviour
{
    [Header("鼠标设置")]
    public Sprite mouseSprite;          // 拖入你的鼠标图片
    public float mouseScale = 0.3f;     // 鼠标大小
    
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    
    void Start()
    {
        // 隐藏系统鼠标
        Cursor.visible = false;
        
        // 获取主摄像机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("❌ 未找到主摄像机！");
            return;
        }
        
        // 设置鼠标外观
        SetupMouseAppearance();
        
        Debug.Log("🎯 游戏鼠标已启动");
    }
    
    void Update()
    {
        // 每帧更新鼠标位置
        UpdateMousePosition();
    }
    
    void SetupMouseAppearance()
    {
        // 添加SpriteRenderer组件
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        
        // 设置鼠标图片
        if (mouseSprite != null)
        {
            spriteRenderer.sprite = mouseSprite;
            transform.localScale = Vector3.one * mouseScale;
            spriteRenderer.sortingOrder = 9999; // 最顶层
        }
        else
        {
            Debug.LogError("❌ 请设置鼠标图片！");
        }
    }
    
    void UpdateMousePosition()
    {
        if (mainCamera == null) return;
        
        // 将屏幕坐标转换为世界坐标
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f; // 确保在摄像机前方
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        
        // 更新鼠标位置
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
    }
    
    // 公开方法：获取鼠标世界位置
    public Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector3.zero;
        
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f;
        return mainCamera.ScreenToWorldPoint(screenPos);
    }
    
    // 公开方法：显示/隐藏鼠标
    public void SetVisible(bool visible)
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = visible;
    }
    
    void OnDestroy()
    {
        // 游戏结束时恢复系统鼠标
        Cursor.visible = true;
    }
}