using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    private GameObject uiPausePanel;
    private bool isPaused = false;
    
    void Start()
    {
        uiPausePanel = GameObject.Find("UIPause");
        if (uiPausePanel == null)
        {
            Debug.LogError("找不到名为'UIPause'的对象！");
            return;
        }
        
        uiPausePanel.SetActive(false);
        
        // 自动绑定按钮事件
        BindButtonEvents();
    }
    
    void BindButtonEvents()
    {
        // 方法1：通过UIPause面板查找子对象
        if (uiPausePanel != null)
        {
            // 在UIPause面板下查找按钮
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
            else
            {
                Debug.LogError("在UIPause下找不到BtnResume按钮！");
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
            else
            {
                Debug.LogError("在UIPause下找不到BtnHome按钮！");
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
    
        // 简单方法：暂停所有AudioListener
        AudioListener.pause = isPaused;
    }

    public void OnResumeClick()
    {
        Time.timeScale = 1f;
        isPaused = false;
        uiPausePanel.SetActive(false);
        AudioListener.pause = false; // 恢复所有声音
    }
    
    public void OnHomeClick()
    {
        Debug.Log("返回主页按钮被点击");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home");
    }
    
}