using System.Collections;
using UnityEngine;
using Utility;


public class Rock : MonoBehaviour
{
    [Range(0f, 1f)] public float volume = 1.0f;
    [Header("销毁设置")]
    public float destroyDelay = 0.5f;  // 🆕 延迟销毁时间
    
    private Vector2 spriteSize;
    private bool isDestroyed = false;  // 🆕 防止重复调用

    void Start()
    {
        // 随机运动
        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        GetComponent<Rigidbody2D>().velocity = direction;
        GetComponent<Rigidbody2D>().angularVelocity = 60f;
    }
    
    void Awake()
    {
        // 确保使用小石头
        spriteSize = transform.Find("SpriteSmall").GetComponent<SpriteRenderer>().size;
        GetComponent<CircleCollider2D>().radius = 0.7f;
    }

    public void PlaySfxRockDestroy()
    {
        if (isDestroyed) return;  // 🆕 防止重复调用
        isDestroyed = true;
        
        Debug.Log("💥 石头被击中，开始销毁流程");
        
        // 1. 播放音效
        PlayDestroySound();
        
        // 2. 禁用碰撞和渲染
        DisableRock();
        
        // 3. 🆕 延迟销毁
        StartCoroutine(DelayedDestroy());
    }
    
    void PlayDestroySound()
    {
        var sfx = Instantiate(transform.Find("SfxRockDestroy"), null);
        sfx.transform.position = transform.position;
        
        AudioSource audioSource = sfx.GetComponent<AudioSource>();
        
        if (AudioManager.Instance != null)
        {
            AudioClip clip = AudioManager.Instance.GetRandomSound();
            
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.Play();
                Debug.Log($"🔊 播放音效: {clip.name}");
                
                // 🆕 音效播放完后销毁音效对象
                Destroy(sfx, clip.length + 0.1f);
            }
        }
    }
    
    void DisableRock()
    {
        // 禁用碰撞
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // 禁用渲染
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        
        // 停止物理
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
        
        Debug.Log("✅ 石头已禁用");
    }
    
    IEnumerator DelayedDestroy()
    {
        // 🆕 等待指定时间
        yield return new WaitForSeconds(destroyDelay);
        
        // 销毁石头
        Destroy(gameObject);
        Debug.Log("🗑️ 石头已销毁");
    }

    void Update()
    {
        if (!isDestroyed)  // 🆕 只有没被摧毁时才更新
        {
            ScreenHelper.RepeatScreen(transform, spriteSize.x, spriteSize.y);
        }
    }
}