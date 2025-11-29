using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
public class UFOSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public float spawnInterval = 20f;      // 生成间隔20秒
    public float startDelay = 5f;          // 开始延迟5秒
    public float ufoSpeed = 3f;           // UFO飞行速度
    
    [Header("生成位置")]
    public float edgeOffset = 2f;          // 屏幕边缘偏移量
    
    [Header("UFO外观")]
    public Sprite ufoSprite;              // UFO图片（可选）
    public Color ufoColor = Color.white;   // UFO颜色
    public float ufoSize = 1f;            // UFO大小
    
    void Start()
    {
        // 开始生成
        InvokeRepeating(nameof(SpawnUFO), startDelay, spawnInterval);
        Debug.Log($"🛸 UFO生成器启动：{startDelay}秒后开始生成");
    }
    
    private void SpawnUFO()
    {
        Vector2 spawnPosition = GetScreenEdgeSpawnPosition();
        GameObject newUFO = CreateUFOFromCode(spawnPosition);
        
        Debug.Log($"🛸 UFO生成成功！位置：{spawnPosition}");
    }
    
    private Vector2 GetScreenEdgeSpawnPosition()
    {
        // 随机选择从左侧或右侧生成
        bool fromRight = Random.Range(0, 2) == 1;
        
        float spawnX = fromRight ? 
            ScreenHelper.RightTopScreenPos.x + edgeOffset :    // 右侧外
            ScreenHelper.LeftBottomScreenPos.x - edgeOffset;    // 左侧外
        
        // 随机Y坐标
        float spawnY = Random.Range(
            ScreenHelper.LeftBottomScreenPos.y + edgeOffset,
            ScreenHelper.RightTopScreenPos.y - edgeOffset
        );
        
        return new Vector2(spawnX, spawnY);
    }
    
    // 🆕 代码创建UFO对象（不依赖预制件）
    private GameObject CreateUFOFromCode(Vector2 position)
    {
        // 1. 创建UFO游戏对象
        GameObject ufo = new GameObject("UFO");
        ufo.transform.position = position;
        ufo.transform.localScale = Vector3.one * ufoSize;
        
        // 2. 添加SpriteRenderer（显示图像）
        SpriteRenderer spriteRenderer = ufo.AddComponent<SpriteRenderer>();
        if (ufoSprite != null)
        {
            spriteRenderer.sprite = ufoSprite;
            spriteRenderer.color = ufoColor;
        }
        else
        {
            // 如果没有图片，创建一个默认图形
            spriteRenderer.color = Color.cyan;
            // 可以在这里添加代码生成基本形状
        }
        
        // 3. 添加刚体（物理）
        Rigidbody2D rb = ufo.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // 4. 添加碰撞体
        CircleCollider2D collider = ufo.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;
        
        // 5. 🆕 添加UFO脚本组件
        UFO ufoScript = ufo.AddComponent<UFO>();
        
        // 6. 设置飞行方向（朝向屏幕中央）
        Vector2 screenCenter = Vector2.zero;
        Vector2 direction = (screenCenter - position).normalized;
        
        // 7. 配置UFO参数
        ufoScript.direction = direction;
        ufoScript.speed = ufoSpeed;
        
        return ufo;
    }
    
    // 手动生成测试
    public void SpawnUFOForTest()
    {
        SpawnUFO();
    }
    
    void Update()
    {
        // 按G键手动生成UFO（测试用）
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnUFOForTest();
            Debug.Log("🎮 手动生成UFO");
        }
    }
}