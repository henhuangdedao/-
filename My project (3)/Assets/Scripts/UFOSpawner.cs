using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utility;

[System.Serializable]
public class UFOType
{
    [Header("UFO类型设置")]
    public string name;               // 类型名称
    public Sprite sprite;             // 你的贴图
    [Tooltip("混响预设选择")]
    public AudioReverbPreset reverbPreset = AudioReverbPreset.Underwater;
}

public class UFOSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public float spawnInterval = 20f;
    public float startDelay = 5f;
    public float ufoSpeed = 3f;
    public float edgeOffset = 2f;
    public float ufoSize = 1f;
    
    [Header("UFO类型配置")]
    [Space(10)]
    public UFOType[] ufoTypes = new UFOType[]
    {
        new UFOType { name = "混响1-黄色UFO", reverbPreset = AudioReverbPreset.Underwater },
        new UFOType { name = "混响2-紫色UFO", reverbPreset = AudioReverbPreset.Cave },
        new UFOType { name = "混响3-橙色UFO", reverbPreset = AudioReverbPreset.Arena },
        new UFOType { name = "混响4-粉色UFO", reverbPreset = AudioReverbPreset.Forest },
        new UFOType { name = "混响5-白色UFO", reverbPreset = AudioReverbPreset.Psychotic }
    };
    
    [Header("音效")]
    public AudioClip[] effectSounds;
    [Range(0f, 1f)] public float soundVolume = 0.8f;
    
    [Header("效果参数")]
    public float effectDuration = 10f;
    [Range(-10000f, 0f)] public float dryLevel = -1000f;
    [Range(-10000f, 2000f)] public float room = 500f;
    
    private int lastUFOIndex = -1;
    
    void Start()
    {
        Debug.Log($"🛸 UFO生成器启动");
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
        try
        {
            Vector2 spawnPosition = GetScreenEdgeSpawnPosition();
            int nextUFOIndex = GetRandomIndexExcludingLast();
            GameObject newUFO = CreateUFOFromCode(spawnPosition, nextUFOIndex);
            
            Debug.Log($"✅ 生成: {ufoTypes[nextUFOIndex].name}");
            lastUFOIndex = nextUFOIndex;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 生成失败: {e.Message}");
        }
    }
    
    private int GetRandomIndexExcludingLast()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < ufoTypes.Length; i++)
        {
            if (i != lastUFOIndex) availableIndices.Add(i);
        }
        return availableIndices.Count == 0 ? Random.Range(0, ufoTypes.Length) : availableIndices[Random.Range(0, availableIndices.Count)];
    }
    
    private GameObject CreateUFOFromCode(Vector2 position, int typeIndex)
    {
        UFOType ufoType = ufoTypes[typeIndex];
        
        GameObject ufo = new GameObject(ufoType.name);
        ufo.transform.position = position;
        ufo.transform.localScale = Vector3.one * ufoSize;
        
        // 只使用你的贴图
        SpriteRenderer spriteRenderer = ufo.AddComponent<SpriteRenderer>();
        if (ufoType.sprite != null)
        {
            spriteRenderer.sprite = ufoType.sprite;
        }
        else
        {
            Debug.LogWarning($"⚠️ {ufoType.name}贴图缺失");
        }
        
        // 物理组件
        Rigidbody2D rb = ufo.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        CircleCollider2D collider = ufo.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;
        
        // UFO脚本
        UFO ufoScript = ufo.AddComponent<UFO>();
        ufoScript.direction = (Vector2.zero - position).normalized;
        ufoScript.speed = ufoSpeed;
        ufoScript.ufoType = typeIndex;
        ufoScript.effectDuration = effectDuration;
        ufoScript.dryLevel = dryLevel;
        ufoScript.room = room;
        ufoScript.effectSounds = effectSounds;
        ufoScript.soundVolume = soundVolume;
        
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
            Debug.LogWarning("⚠️ 使用默认坐标");
        }
        
        bool fromRight = Random.Range(0, 2) == 1;
        
        float spawnX = fromRight ? 
            rightTop.x + edgeOffset : 
            leftBottom.x - edgeOffset;
        
        float spawnY = Random.Range(leftBottom.y + edgeOffset, rightTop.y - edgeOffset);
        
        return new Vector2(spawnX, spawnY);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUFO();
        }
    }
}