using UnityEngine;
using Utility;

public class RockSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public int maxRockCount = 30;
    public int rocksPerSpawn = 5;
    public float spawnInterval = 1f;
    
    private float timer = 0f;

    void Start()
    {
        Debug.Log("🎮 石头生成器启动");
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
            
            Debug.Log($"⏰ 生成{spawnCount}个石头，当前{currentCount + spawnCount}/30");
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
            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rb.velocity = dir * Random.Range(2f, 5f);
            rb.angularVelocity = Random.Range(30f, 90f);
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
                y = ScreenHelper.RightTopScreenPos.y + 2f;
                break;
            case 1: // 右
                x = ScreenHelper.RightTopScreenPos.x + 2f;
                y = Random.Range(ScreenHelper.LeftBottomScreenPos.y, ScreenHelper.RightTopScreenPos.y);
                break;
            case 2: // 下
                x = Random.Range(ScreenHelper.LeftBottomScreenPos.x, ScreenHelper.RightTopScreenPos.x);
                y = ScreenHelper.LeftBottomScreenPos.y - 2f;
                break;
            case 3: // 左
                x = ScreenHelper.LeftBottomScreenPos.x - 2f;
                y = Random.Range(ScreenHelper.LeftBottomScreenPos.y, ScreenHelper.RightTopScreenPos.y);
                break;
        }
        
        return new Vector2(x, y);
    }
}