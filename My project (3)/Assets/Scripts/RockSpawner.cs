using UnityEngine;
using Utility;

public class RockSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public int maxRockCount = 30;      // ✅ 保持30个上限
    public int rocksPerSpawn = 8;      // 🆕 增加到8个（原5个）
    public float spawnInterval = 0.5f; // 🆕 减小到0.5秒（原1秒）
    
    [Header("生成位置")]
    public float edgeOffset = 1f;      // 🆕 边缘偏移量
    
    [Header("速度范围")]
    public float minSpeed = 3f;        // 🆕 最小速度
    public float maxSpeed = 6f;        // 🆕 最大速度
    
    [Header("角度范围")]  
    public float minRotation = 40f;    // 🆕 最小旋转
    public float maxRotation = 100f;   // 🆕 最大旋转
    
    private float timer = 0f;

    void Start()
    {
        Debug.Log($"🎮 石头生成器启动");
        Debug.Log($"生成间隔: {spawnInterval}秒, 每次生成: {rocksPerSpawn}个");
        
        // 🆕 游戏开始立即生成一批石头
        SpawnInitialRocks();
    }
    
    // 🆕 初始生成
    void SpawnInitialRocks()
    {
        int spawnCount = Mathf.Min(rocksPerSpawn * 2, maxRockCount);
        
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRockAtEdge();
        }
        
        Debug.Log($"🎯 初始生成{spawnCount}个石头");
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRocksIfNeeded();
        }
    }
    
    void SpawnRocksIfNeeded()
    {
        int currentCount = FindObjectsOfType<Rock>().Length;
        
        if (currentCount < maxRockCount)
        {
            int spawnCount = Mathf.Min(rocksPerSpawn, maxRockCount - currentCount);
            
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnRockAtEdge();
            }
            
            Debug.Log($"⏰ 生成{spawnCount}个石头，当前{currentCount + spawnCount}/{maxRockCount}");
        }
    }
    
    void SpawnRockAtEdge()
    {
        Rock[] rocks = FindObjectsOfType<Rock>();
        if (rocks.Length == 0) 
        {
            Debug.LogError("❌ 找不到石头预制体！");
            return;
        }
        
        Vector2 spawnPos = GetScreenEdgePosition();
        Rock newRock = Instantiate(rocks[0], spawnPos, Quaternion.identity);
        
        Rigidbody2D rb = newRock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 🆕 从边缘向屏幕中心发射
            Vector2 screenCenter = Vector2.zero;
            Vector2 direction = (screenCenter - spawnPos).normalized;
            
            // 🆕 添加一点随机偏移
            float angleOffset = Random.Range(-30f, 30f);
            direction = Quaternion.Euler(0, 0, angleOffset) * direction;
            
            float speed = Random.Range(minSpeed, maxSpeed);
            rb.velocity = direction * speed;
            rb.angularVelocity = Random.Range(minRotation, maxRotation);
        }
    }
    
    Vector2 GetScreenEdgePosition()
    {
        int edge = Random.Range(0, 4);
        float x = 0f, y = 0f;
        
        switch (edge)
        {
            case 0: // 上
                x = Random.Range(ScreenHelper.LeftBottomScreenPos.x, ScreenHelper.RightTopScreenPos.x);
                y = ScreenHelper.RightTopScreenPos.y + edgeOffset;
                break;
            case 1: // 右
                x = ScreenHelper.RightTopScreenPos.x + edgeOffset;
                y = Random.Range(ScreenHelper.LeftBottomScreenPos.y, ScreenHelper.RightTopScreenPos.y);
                break;
            case 2: // 下
                x = Random.Range(ScreenHelper.LeftBottomScreenPos.x, ScreenHelper.RightTopScreenPos.x);
                y = ScreenHelper.LeftBottomScreenPos.y - edgeOffset;
                break;
            case 3: // 左
                x = ScreenHelper.LeftBottomScreenPos.x - edgeOffset;
                y = Random.Range(ScreenHelper.LeftBottomScreenPos.y, ScreenHelper.RightTopScreenPos.y);
                break;
        }
        
        return new Vector2(x, y);
    }
    
    // 🆕 编辑器调试
    [ContextMenu("立即生成一批石头")]
    public void SpawnBatchImmediate()
    {
        int spawnCount = Mathf.Min(rocksPerSpawn, maxRockCount - FindObjectsOfType<Rock>().Length);
        
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRockAtEdge();
        }
        
        Debug.Log($"⚡ 立即生成{spawnCount}个石头");
    }
}