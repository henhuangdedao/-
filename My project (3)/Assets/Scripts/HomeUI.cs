using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class HomeUI : MonoBehaviour
{
    [Header("场景设置")]
    public string instructionsSceneName = "InstructionsScene"; // Instructions场景名
    public float fadeOutTime = 0.5f; // 淡出时间
    
    void Start()
    {
        // 查找开始按钮
        Button btnStart = FindStartButton();
        if (btnStart != null)
        {
            btnStart.onClick.AddListener(OnStartButtonClick);
            Debug.Log("✅ 开始按钮事件绑定成功！");
        }
        else
        {
            Debug.LogError("❌ 找不到开始按钮！");
        }
    }
    
    Button FindStartButton()
    {
        // 方法1：通过transform.Find查找
        Transform btnTransform = transform.Find("BtnStart");
        if (btnTransform == null)
        {
            // 方法2：通过GameObject.Find查找
            GameObject btnObj = GameObject.Find("BtnStart");
            if (btnObj != null) btnTransform = btnObj.transform;
        }
        if (btnTransform == null)
        {
            // 方法3：通过标签查找
            GameObject btnObj = GameObject.FindGameObjectWithTag("StartButton");
            if (btnObj != null) btnTransform = btnObj.transform;
        }
        
        return btnTransform?.GetComponent<Button>();
    }
    
    void OnStartButtonClick()
    {
        Debug.Log("🚀 开始游戏！加载Instructions场景");
        
        // 禁用按钮防止重复点击
        Button btnStart = FindStartButton();
        if (btnStart != null) btnStart.interactable = false;
        
        // 添加淡出效果并加载Instructions场景
        StartCoroutine(FadeOutAndLoadInstructions());
    }
    
    IEnumerator FadeOutAndLoadInstructions()
    {
        // 获取或添加CanvasGroup
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 淡出效果
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        
        // 加载Instructions场景
        SceneManager.LoadScene("Instruction");
    }
    
    // 可选：添加键盘快捷键
    void Update()
    {
        // 按回车键也可以开始游戏
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            OnStartButtonClick();
        }
    }
}