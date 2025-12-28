using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class UIPause : MonoBehaviour
{
    private GameObject uiPausePanel;
    private bool isPaused = false;
    
    [Header("淡入设置")]
    public float fadeInTime = 0.5f;
    public bool enableFadeEffect = true;
    
    void Start()
    {
        uiPausePanel = GameObject.Find("UIPause");
        if (uiPausePanel == null)
        {
            Debug.LogError("找不到名为'UIPause'的对象！");
            return;
        }
        
        uiPausePanel.SetActive(false);
        BindButtonEvents();
    }
    
    void BindButtonEvents()
    {
        if (uiPausePanel != null)
        {
            Transform btnResumeTransform = uiPausePanel.transform.Find("BtnResume");
            Transform btnHomeTransform = uiPausePanel.transform.Find("BtnHome");
            
            if (btnResumeTransform != null)
            {
                Button btnResume = btnResumeTransform.GetComponent<Button>();
                if (btnResume != null)
                {
                    btnResume.onClick.AddListener(OnResumeClick);
                    Debug.Log("继续按钮事件绑定成功！");
                }
            }
            
            if (btnHomeTransform != null)
            {
                Button btnHome = btnHomeTransform.GetComponent<Button>();
                if (btnHome != null)
                {
                    btnHome.onClick.AddListener(OnHomeClick);
                    Debug.Log("主页按钮事件绑定成功！");
                }
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    void TogglePause()
    {
        if (uiPausePanel == null) return;
    
        isPaused = !isPaused;
        uiPausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }

    public void OnResumeClick()
    {
        Time.timeScale = 1f;
        isPaused = false;
        uiPausePanel.SetActive(false);
        AudioListener.pause = false;
    }
    
    public void OnHomeClick()
    {
        Debug.Log("返回主页按钮被点击");
        
        // 1. 恢复游戏状态
        Time.timeScale = 1f;
        isPaused = false;
        uiPausePanel.SetActive(false);
        AudioListener.pause = false;
        
        // 2. 🆕 重置AudioManager到默认音效
        ResetAudioManager();
        
        // 3. 🆕 重置背景到默认
        ResetBackground();
        
        // 4. 直接加载主页场景
        SceneManager.LoadScene("Home");
    }
    
    // 🆕 重置AudioManager
    void ResetAudioManager()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResetToDefault();
            Debug.Log("✅ 重置AudioManager为默认音效");
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager.Instance为null，无法重置");
        }
    }
    
    // 🆕 重置背景
    void ResetBackground()
    {
        // 查找背景管理器
        ShaderBackgroundManager bgManager = FindObjectOfType<ShaderBackgroundManager>();
        if (bgManager != null)
        {
            bgManager.ResetToDefault();
            Debug.Log("✅ 重置背景为默认");
        }
        else
        {
            Debug.LogWarning("⚠️ 找不到ShaderBackgroundManager");
        }
    }
}