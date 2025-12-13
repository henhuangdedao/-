using UnityEngine;
using Utility;

public class Rock : MonoBehaviour
{
    public AudioClip[] destroySounds;
    [Range(0f, 1f)] public float volume = 1.0f;
    
    private Vector2 spriteSize;

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
        var sfx = Instantiate(transform.Find("SfxRockDestroy"), null);
        sfx.transform.position = transform.position;
        
        AudioSource audioSource = sfx.GetComponent<AudioSource>();
        if (destroySounds != null && destroySounds.Length > 0)
        {
            int randomIndex = Random.Range(0, destroySounds.Length);
            audioSource.clip = destroySounds[randomIndex];
        }
        audioSource.volume = volume;
        audioSource.Play();
    }

    void Update()
    {
        // 屏幕循环
        ScreenHelper.RepeatScreen(transform, spriteSize.x, spriteSize.y);
    }
}