using UnityEngine;
using Utility;
using System.Collections;



public class UFO : MonoBehaviour
{
    [Header("基本设置")]
    public Vector2 direction = Vector2.left;
    public float speed = 3f;
    public int ufoType = 0;
    
    [Header("音效")]
    public AudioClip hitSound;
    [Range(0f, 1f)] public float volume = 0.8f;
    
    // 🆕 删除public backgroundManager字段
    private ShaderBackgroundManager bgManager;
    private Rigidbody2D rb;
    private bool isDestroyed = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * speed;
        }
        
        // 🆕 自动查找背景管理器
        bgManager = FindObjectOfType<ShaderBackgroundManager>();
        if (bgManager != null)
        {
            Debug.Log($"✅ UFO找到背景管理器");
        }
        
        Debug.Log($"✅ UFO生成: 类型{ufoType}");
    }
    
    void ApplyEffects()
    {
        Debug.Log($"🎯 UFO{ufoType}被击中");
        
        // 1. 切换背景
        if (bgManager != null)
        {
            bgManager.SwitchToUFOBackground(ufoType);
        }
        else
        {
            Debug.LogError("❌ 背景管理器为null");
        }
        
        // 2. 切换音频
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SwitchToUFO(ufoType);
        }
        
        // 3. 播放UFO被击中音效
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position, volume);
        }
    }
    void Update()
    {
        if (rb != null && !isDestroyed) rb.velocity = direction * speed;
        ScreenHelper.RepeatScreen(transform, 1f, 1f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed) return;
        
        if (other.CompareTag("Bullet"))
        {
            isDestroyed = true;
            
            // 1. 禁用UFO
            DisableUFO();
            
            // 2. 应用效果
            ApplyEffects();
            
            // 3. 加分
            if (Game.Default != null) Game.AddScore(100);
            
            // 4. 延迟销毁
            Destroy(gameObject, 1f);
        }
    }
    
    void DisableUFO()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        
        if (rb != null) rb.velocity = Vector2.zero;
    }
    
   
}