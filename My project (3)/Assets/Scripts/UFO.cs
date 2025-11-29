using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;


public class UFO : MonoBehaviour
{
    [Header("飞行设置")]
    public Vector2 direction = Vector2.left;
    public float speed = 3f;
    
    [Header("💓 心跳音效")]
    public AudioClip heartbeatSound;
    [Range(0f, 1f)] public float heartbeatVolume = 0.8f;
    
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 确保方向已标准化
        direction = direction.normalized;
        
        // 立即开始飞行
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * speed;
        }
        
        Debug.Log($"🛸 UFO启动：方向{direction}, 速度{speed}");
    }
    
    void Update()
    {
        // 保持飞行速度
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        
        // 屏幕循环
        ScreenHelper.RepeatScreen(transform, 1f, 1f);
        
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyHeartbeatEffect();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyRetroEffect();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ApplySpaceEffect();
        if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyGlitchEffect();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") || other.GetComponent<Bullet>() != null)
        {
            ApplyRandomEffect();
            Destroy(gameObject);
            if (Game.Default != null) Game.AddScore(100);
        }
    }
    
    void ApplyRandomEffect()
    {
        if (Camera.main == null) return;
        
        int effectType = Random.Range(0, 4);
        
        switch(effectType)
        {
            case 0: ApplyHeartbeatEffect(); break;
            case 1: ApplyRetroEffect(); break;
            case 2: ApplySpaceEffect(); break;
            case 3: ApplyGlitchEffect(); break;
        }
    }
    
    // 🫀 1. 心脏效果
    void ApplyHeartbeatEffect()
    {
        if (Camera.main == null) return;
        
        RemoveExistingFilters();
        
        AudioLowPassFilter lowPass = GetOrAddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = 120f;
        lowPass.lowpassResonanceQ = 3.0f;
        
        AudioEchoFilter echo = GetOrAddComponent<AudioEchoFilter>();
        echo.delay = 500f;
        echo.wetMix = 0.7f;
        echo.dryMix = 0.3f;
        
        Destroy(lowPass, 10f);
        Destroy(echo, 10f);
        
        // 播放心跳音效
        PlayHeartbeatSound();
        
        Debug.Log("💓 心脏效果激活");
    }
    
    void PlayHeartbeatSound()
    {
        if (heartbeatSound == null) return;
        
        AudioSource.PlayClipAtPoint(heartbeatSound, Camera.main.transform.position, heartbeatVolume);
        Debug.Log("💓 心跳音效播放");
    }
    
    // 🎮 2. 复古8-bit效果
    void ApplyRetroEffect()
    {
        if (Camera.main == null) return;
        
        RemoveExistingFilters();
        
        AudioLowPassFilter lowPass = GetOrAddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = 3000f;
        lowPass.lowpassResonanceQ = 1.5f;
        
        AudioDistortionFilter distortion = GetOrAddComponent<AudioDistortionFilter>();
        distortion.distortionLevel = 0.6f;
        
        Destroy(lowPass, 10f);
        Destroy(distortion, 10f);
        
        Debug.Log("🎮 复古8-bit效果激活");
    }
    
    // 🚀 3. 太空迷幻效果
    void ApplySpaceEffect()
    {
        if (Camera.main == null) return;
        
        RemoveExistingFilters();
        
        AudioReverbFilter reverb = GetOrAddComponent<AudioReverbFilter>();
        reverb.reverbPreset = AudioReverbPreset.Underwater;
        reverb.dryLevel = -1500;
        
        AudioChorusFilter chorus = GetOrAddComponent<AudioChorusFilter>();
        chorus.dryMix = 0.2f;
        chorus.wetMix1 = 0.9f;
        chorus.rate = 0.6f;
        chorus.depth = 0.4f;
        
        Destroy(reverb, 12f);
        Destroy(chorus, 12f);
        
        Debug.Log("🚀 太空迷幻效果激活");
    }
    
    // 💥 4. 数字故障效果
    void ApplyGlitchEffect()
    {
        if (Camera.main == null) return;
        
        RemoveExistingFilters();
        
        AudioHighPassFilter highPass = GetOrAddComponent<AudioHighPassFilter>();
        highPass.cutoffFrequency = 1500f;
        highPass.highpassResonanceQ = 4.0f;
        
        AudioEchoFilter echo = GetOrAddComponent<AudioEchoFilter>();
        echo.delay = 150f;
        echo.wetMix = 0.8f;
        
        Destroy(highPass, 8f);
        Destroy(echo, 8f);
        
        Debug.Log("💥 数字故障效果激活");
    }
    
    // 🧹 清理效果器
    void RemoveExistingFilters()
    {
        if (Camera.main == null) return;
        
        AudioDistortionFilter[] distortions = Camera.main.GetComponents<AudioDistortionFilter>();
        AudioLowPassFilter[] lowPasses = Camera.main.GetComponents<AudioLowPassFilter>();
        AudioHighPassFilter[] highPasses = Camera.main.GetComponents<AudioHighPassFilter>();
        AudioReverbFilter[] reverbs = Camera.main.GetComponents<AudioReverbFilter>();
        AudioChorusFilter[] choruses = Camera.main.GetComponents<AudioChorusFilter>();
        AudioEchoFilter[] echoes = Camera.main.GetComponents<AudioEchoFilter>();
        
        foreach (var filter in distortions) Destroy(filter);
        foreach (var filter in lowPasses) Destroy(filter);
        foreach (var filter in highPasses) Destroy(filter);
        foreach (var filter in reverbs) Destroy(filter);
        foreach (var filter in choruses) Destroy(filter);
        foreach (var filter in echoes) Destroy(filter);
    }
    
    // 🔧 获取或添加组件
    T GetOrAddComponent<T>() where T : Component
    {
        T component = Camera.main.GetComponent<T>();
        if (component == null)
        {
            component = Camera.main.gameObject.AddComponent<T>();
        }
        return component;
    }
}