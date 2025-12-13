using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIPause : MonoBehaviour
{
    private GameObject uiPausePanel;
    private bool isPaused = false;
    
    [Header("淡入设置")]
    public float fadeInTime = 0.5f; // 主页的淡入时间
    public bool enableFadeEffect = true; // 是否启用淡入效果
    
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
        
        // 恢复游戏状态
        Time.timeScale = 1f;
        isPaused = false;
        uiPausePanel.SetActive(false);
        AudioListener.pause = false;
        
        // 直接加载主页场景（Home场景会处理自己的淡入效果）
        SceneManager.LoadScene("Home");
    }
}