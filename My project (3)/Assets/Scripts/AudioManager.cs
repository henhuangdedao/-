using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("音频数组")]
    public AudioClip[] defaultSounds;     // 默认音效
    public AudioClip[] ufo0Sounds;        // 黄色UFO音效
    public AudioClip[] ufo1Sounds;        // 紫色UFO音效  
    public AudioClip[] ufo2Sounds;        // 橙色UFO音效
    public AudioClip[] ufo3Sounds;        // 粉色UFO音效
    
    [Header("当前状态")]
    [SerializeField] private AudioClip[] currentSounds;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 订阅场景加载事件
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // 初始化
            Initialize();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    // 初始化/重置方法
    public void Initialize()
    {
        currentSounds = defaultSounds;
        
        Debug.Log($"✅ AudioManager初始化");
        Debug.Log($"默认音效: {defaultSounds?.Length ?? 0}个");
    }
    
    // 场景加载时的回调
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🔄 场景加载: {scene.name}");
        
        // 如果是游戏场景，重置到默认
        // 注意：将"Game"替换为你的游戏场景名称
        if (scene.name == "Game" || scene.name.Contains("Game"))
        {
            ResetToDefault();
        }
    }
    
    // 切换到指定UFO音效
    public void SwitchToUFO(int ufoType)
    {
        Debug.Log($"🔄 AudioManager切换到UFO{ufoType}");
        
        switch (ufoType)
        {
            case 0: 
                currentSounds = ufo0Sounds;
                Debug.Log($"使用黄色UFO音效: {ufo0Sounds?.Length ?? 0}个");
                break;
            case 1: 
                currentSounds = ufo1Sounds;
                Debug.Log($"使用紫色UFO音效: {ufo1Sounds?.Length ?? 0}个");
                break;
            case 2: 
                currentSounds = ufo2Sounds;
                Debug.Log($"使用橙色UFO音效: {ufo2Sounds?.Length ?? 0}个");
                break;
            case 3: 
                currentSounds = ufo3Sounds;
                Debug.Log($"使用粉色UFO音效: {ufo3Sounds?.Length ?? 0}个");
                break;
            default:
                Debug.LogError($"❌ 无效UFO类型: {ufoType}");
                return;
        }
        
        // 检查切换是否成功
        if (currentSounds == null || currentSounds.Length == 0)
        {
            Debug.LogError($"❌ UFO{ufoType}音效数组无效，回退到默认");
            currentSounds = defaultSounds;
        }
    }
    
    // 重置为默认音频
    public void ResetToDefault()
    {
        currentSounds = defaultSounds;
        Debug.Log($"✅ AudioManager重置为默认音效: {currentSounds?.Length ?? 0}个");
    }
    
    // 获取随机音效
    public AudioClip GetRandomSound()
    {
        if (currentSounds != null && currentSounds.Length > 0)
        {
            int index = Random.Range(0, currentSounds.Length);
            AudioClip clip = currentSounds[index];
            
            if (clip != null)
            {
                return clip;
            }
            else
            {
                Debug.LogError($"❌ 音效索引{index}为null");
            }
        }
        else
        {
            Debug.LogError($"❌ 当前音效数组: 存在={currentSounds != null}, 长度={currentSounds?.Length ?? 0}");
        }
        
        return null;
    }
    
    // 获取当前音效数量
    public int GetCurrentSoundCount()
    {
        return currentSounds != null ? currentSounds.Length : 0;
    }
    
    // 获取当前使用的数组名称
    public string GetCurrentArrayName()
    {
        if (currentSounds == defaultSounds) return "默认";
        if (currentSounds == ufo0Sounds) return "黄色UFO";
        if (currentSounds == ufo1Sounds) return "紫色UFO";
        if (currentSounds == ufo2Sounds) return "橙色UFO";
        if (currentSounds == ufo3Sounds) return "粉色UFO";
        return "未知";
    }
    
    // 显示当前状态
    void OnGUI()
    {
        if (currentSounds != null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"当前音频: {GetCurrentArrayName()}");
            GUI.Label(new Rect(10, 30, 300, 20), $"音效数量: {currentSounds.Length}个");
        }
    }
    
    // 清理
    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    // 🆕 编辑器调试：按R键重置
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("🎮 按R键重置AudioManager");
            ResetToDefault();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("=== AudioManager状态检查 ===");
            Debug.Log($"当前数组: {GetCurrentArrayName()}");
            Debug.Log($"音效数量: {GetCurrentSoundCount()}");
        }
    }
}