using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InstructionsSceneController : MonoBehaviour
{
    [Header("场景设置")]
    public string gameSceneName = "Game";
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    
    private CanvasGroup mainCanvasGroup;
    private bool isReadyForInput = false;
    
    void Start()
    {
        Debug.Log("📋 操作说明场景已加载");
        
        // 🆕 自动查找并设置CanvasGroup
        SetupCanvasGroup();
        StartCoroutine(FadeInAndEnableInput());
    }
    
    void SetupCanvasGroup()
    {
        // 查找Canvas
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            Debug.LogError("❌ 找不到Canvas对象！");
            return;
        }
        
        // 获取或添加CanvasGroup
        mainCanvasGroup = canvasObj.GetComponent<CanvasGroup>();
        if (mainCanvasGroup == null)
        {
            mainCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
            Debug.Log("✅ 已为Canvas添加CanvasGroup组件");
        }
        else
        {
            Debug.Log("✅ 找到CanvasGroup组件");
        }
    }
    
    IEnumerator FadeInAndEnableInput()
    {
        if (mainCanvasGroup == null)
        {
            Debug.LogError("❌ CanvasGroup未设置，跳过淡入效果");
            isReadyForInput = true;
            yield break;
        }
        
        mainCanvasGroup.alpha = 0f;
        Debug.Log("🎬 开始淡入效果");
        
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            mainCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInTime);
            yield return null;
        }
        
        mainCanvasGroup.alpha = 1f;
        isReadyForInput = true;
        Debug.Log("✅ 淡入完成");
    }
    
    IEnumerator FadeOutAndLoadGame()
    {
        if (mainCanvasGroup == null)
        {
            SceneManager.LoadScene(gameSceneName);
            yield break;
        }
        
        isReadyForInput = false;
        Debug.Log("🎬 开始淡出效果");
        
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            mainCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            yield return null;
        }
        
        mainCanvasGroup.alpha = 0f;
        SceneManager.LoadScene(gameSceneName);
    }
    
    void Update()
    {
        if (!isReadyForInput) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadeOutAndLoadGame());
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Home");
        }
    }
}