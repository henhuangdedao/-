using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

// 🎯 UFOType类定义在最开头
[System.Serializable]
public class UFOType
{
    [Header("UFO类型设置")]
    public string name;
    public Sprite sprite;
}

// 🎯 UFOSpawner类
public class UFOSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public float spawnInterval = 20f;
    public float startDelay = 5f;
    public float ufoSpeed = 3f;
    public float edgeOffset = 2f;
    public float ufoSize = 1f;
    
    [Header("UFO类型配置 (4个)")]
    [Space(10)]
    public UFOType[] ufoTypes = new UFOType[4]
    {
        new UFOType { name = "黄色UFO-水下" },
        new UFOType { name = "紫色UFO-洞穴" },
        new UFOType { name = "橙色UFO-竞技场" },
        new UFOType { name = "粉色UFO-森林" }
    };
    
    private int lastUFOIndex = -1;
    
    void Start()
    {
        Debug.Log($"🛸 UFO生成器启动 (4种类型)");
        StartCoroutine(SpawnUFOCoroutine());
    }
    
    IEnumerator SpawnUFOCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (true)
        {
            SpawnUFO();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private void SpawnUFO()
    {
        Vector2 spawnPosition = GetScreenEdgeSpawnPosition();
        int nextUFOIndex = GetRandomIndexExcludingLast();
        GameObject newUFO = CreateUFO(spawnPosition, nextUFOIndex);
        
        lastUFOIndex = nextUFOIndex;
    }
    
    private int GetRandomIndexExcludingLast()
    {
        List<int> available = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i != lastUFOIndex) available.Add(i);
        }
        return available.Count == 0 ? Random.Range(0, 4) : available[Random.Range(0, available.Count)];
    }
    
    private GameObject CreateUFO(Vector2 position, int typeIndex)
    {
        UFOType ufoType = ufoTypes[typeIndex];
        
        GameObject ufo = new GameObject(ufoType.name);
        ufo.transform.position = position;
        ufo.transform.localScale = Vector3.one * ufoSize;
        
        // 贴图
        SpriteRenderer spriteRenderer = ufo.AddComponent<SpriteRenderer>();
        if (ufoType.sprite != null) spriteRenderer.sprite = ufoType.sprite;
        
        // 物理
        Rigidbody2D rb = ufo.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        
        CircleCollider2D collider = ufo.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;
        
        // UFO脚本
        UFO ufoScript = ufo.AddComponent<UFO>();
        ufoScript.ufoType = typeIndex;
        ufoScript.direction = (Vector2.zero - position).normalized;
        ufoScript.speed = ufoSpeed;
        
        return ufo;
    }
    
    private Vector2 GetScreenEdgeSpawnPosition()
    {
        Vector2 leftBottom = new Vector2(-10f, -6f);
        Vector2 rightTop = new Vector2(10f, 6f);
        
        try
        {
            if (ScreenHelper.LeftBottomScreenPos != Vector2.zero)
            {
                leftBottom = ScreenHelper.LeftBottomScreenPos;
                rightTop = ScreenHelper.RightTopScreenPos;
            }
        }
        catch
        {
            // 使用默认坐标
        }
        
        bool fromRight = Random.Range(0, 2) == 1;
        float spawnX = fromRight ? rightTop.x + edgeOffset : leftBottom.x - edgeOffset;
        float spawnY = Random.Range(leftBottom.y + edgeOffset, rightTop.y - edgeOffset);
        
        return new Vector2(spawnX, spawnY);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SpawnUFO();
    }
}