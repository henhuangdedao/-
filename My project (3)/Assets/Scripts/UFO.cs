using UnityEngine;
using Utility;

public class UFO : MonoBehaviour
{
    [Header("飞行设置")]
    public Vector2 direction = Vector2.left;
    public float speed = 3f;
    
    [Header("UFO类型")]
    public int ufoType = 0;
    
    [Header("效果设置")]
    public float effectDuration = 10f;
    [Range(-10000f, 0f)] public float dryLevel = -1000f;
    [Range(-10000f, 2000f)] public float room = 500f;
    
    [Header("音效")]
    public AudioClip[] effectSounds;
    [Range(0f, 1f)] public float soundVolume = 0.8f;
    
    private Rigidbody2D rb;
    private UFOSpawner spawner;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawner = FindObjectOfType<UFOSpawner>();
        direction = direction.normalized;
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * speed;
        }
    }
    
    void Update()
    {
        if (rb != null) rb.velocity = direction * speed;
        ScreenHelper.RepeatScreen(transform, 1f, 1f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") || other.GetComponent<Bullet>() != null)
        {
            ApplyReverbEffect();
            Destroy(gameObject);
            if (Game.Default != null) Game.AddScore(100);
        }
    }
    
    void ApplyReverbEffect()
    {
        if (Camera.main == null || spawner == null) return;
        
        AudioReverbPreset preset = spawner.ufoTypes[Mathf.Clamp(ufoType, 0, spawner.ufoTypes.Length - 1)].reverbPreset;
        
        AudioReverbFilter reverb = GetOrAddComponent<AudioReverbFilter>();
        reverb.reverbPreset = preset;
        reverb.dryLevel = dryLevel;
        reverb.room = room;
        
        Destroy(reverb, effectDuration);
        
        PlayEffectSound();
    }
    
    void PlayEffectSound()
    {
        if (effectSounds == null || effectSounds.Length == 0) return;
        
        int soundIndex = Mathf.Clamp(ufoType, 0, effectSounds.Length - 1);
        
        if (effectSounds[soundIndex] != null)
        {
            AudioSource.PlayClipAtPoint(effectSounds[soundIndex], Camera.main.transform.position, soundVolume);
        }
    }
    
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