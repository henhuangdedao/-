using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class HomeUI : MonoBehaviour
{
    [Header("UI元素")]
    public RectTransform guideImage;
    
    [Header("动画设置")]
    public float moveDownTime = 1f;
    public float moveUpTime = 1f;
    
    [Header("场景设置")]
    public string gameSceneName = "Game";
    
    private bool isTransitioning = false;
    private Canvas guideCanvas;
    private GameObject homeEventSystem;
    
    void Start()
    {
        // 确保自己不销毁
        DontDestroyOnLoad(gameObject);
        
        if (guideImage != null)
        {
            guideCanvas = guideImage.GetComponentInParent<Canvas>();
            if (guideCanvas != null)
            {
                DontDestroyOnLoad(guideCanvas.gameObject);
                guideCanvas.sortingOrder = 999;
            }
            
            guideImage.anchoredPosition = new Vector2(0, 1080f);
        }
        
        // 记录Home场景的EventSystem
        homeEventSystem = GameObject.Find("EventSystem");
    }
    
    public void StartGameTransition()
    {
        if (isTransitioning || guideImage == null) return;
        
        StartCoroutine(GameTransitionCoroutine());
    }
    
    IEnumerator GameTransitionCoroutine()
    {
        isTransitioning = true;
        
        Debug.Log("🚀 开始游戏过渡");
        
        // 1. 下移Guide
        yield return StartCoroutine(MoveGuide(1080f, 0f, moveDownTime));
        
        Debug.Log("✅ Guide下移完成，等待空格键");
        
        // 2. 等待空格键
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        
        Debug.Log("🎮 空格键按下，准备切换");
        
        // 3. 🎯 隐藏Home场景
        HideHomeSceneCompletely();
        
        // 4. 🎯 销毁Home的EventSystem
        if (homeEventSystem != null)
        {
            Destroy(homeEventSystem);
            homeEventSystem = null;
            Debug.Log("✅ 销毁Home EventSystem");
        }
        
        // 5. 等待一帧
        yield return null;
        
        // 6. 加载Game场景
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        
        // 7. 等待场景加载
        yield return null;
        
        // 8. 🎯 确保只有一个EventSystem
        CleanupDuplicateEventSystems();
        
        // 9. 上移Guide
        Debug.Log("⬆️ 开始上移Guide");
        yield return StartCoroutine(MoveGuide(0f, 1080f, moveUpTime));
        
        Debug.Log("🎉 过渡完成！");
        
        isTransitioning = false;
    }
    
    // 🎯 清理重复的EventSystem
    void CleanupDuplicateEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning($"发现{eventSystems.Length}个EventSystem，清理中...");
            
            // 保留第一个，销毁其他的
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
            
            Debug.Log($"✅ 清理完成，保留1个EventSystem");
        }
        else if (eventSystems.Length == 0)
        {
            Debug.Log("⚠️ 没有EventSystem，正在创建...");
            CreateEventSystem();
        }
    }
    
    // 🎯 创建EventSystem
    void CreateEventSystem()
    {
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();
        DontDestroyOnLoad(eventSystemObj);
        Debug.Log("✅ 创建了新的EventSystem");
    }
    
    // 🎯 隐藏Home场景
    void HideHomeSceneCompletely()
    {
        // 禁用Parallax
        Parallax[] parallaxScripts = FindObjectsOfType<Parallax>();
        foreach (Parallax parallax in parallaxScripts)
        {
            parallax.enabled = false;
            if (parallax.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.enabled = false;
            }
        }
        
        // 隐藏不是GuideCanvas的Canvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas == guideCanvas) continue;
            canvas.gameObject.SetActive(false);
        }
        
        Debug.Log("✅ Home场景已隐藏");
    }
    
    IEnumerator MoveGuide(float fromY, float toY, float duration)
    {
        if (guideImage == null) yield break;
        
        float timer = 0f;
        Vector2 startPos = new Vector2(0, fromY);
        Vector2 endPos = new Vector2(0, toY);
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = Mathf.SmoothStep(0, 1, t);
            
            guideImage.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        
        guideImage.anchoredPosition = endPos;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGameTransition();
        }
    }
}