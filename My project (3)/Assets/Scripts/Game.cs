using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public static Game Default;
    public static int Score = 0;
    public static int HighScore = 0;
    Text mScoreText;
    
    private GameObject uiGameOver; // 游戏结束界面

    private void Awake()
    {
        Default = this;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // 确保游戏开始时时间正常
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void OnDestroy()
    {
        Default = null;
    }

    private void Start()
    {
        mScoreText = transform.Find("UI/ScoreText").GetComponent<Text>();
        Score = 0;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreText();
        
        // 确保Start时时间正常
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        // 查找游戏结束界面
        uiGameOver = GameObject.Find("UIGameOver");
        if (uiGameOver != null)
        {
            uiGameOver.SetActive(false); // 初始隐藏
            BindGameOverButtons(); // 绑定按钮事件
        }
        else
        {
            Debug.LogWarning("找不到UIGameOver对象！");
        }
    }
    
    void BindGameOverButtons()
    {
        // 绑定重新开始按钮
        Transform btnRestart = uiGameOver.transform.Find("BtnRestart");
        if (btnRestart != null)
        {
            Button restartButton = btnRestart.GetComponent<Button>();
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
                Debug.Log("重新开始按钮绑定成功！");
            }
        }
        
        // 绑定返回主界面按钮
        Transform btnHome = uiGameOver.transform.Find("BtnHome");
        if (btnHome != null)
        {
            Button homeButton = btnHome.GetComponent<Button>();
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(ReturnToHome);
                Debug.Log("返回主界面按钮绑定成功！");
            }
        }
    }
    
    public static void ShowGameOver()
    {
        if (Default != null && Default.uiGameOver != null)
        {
            Default.uiGameOver.SetActive(true); // 显示游戏结束界面
            Time.timeScale = 0f; // 暂停游戏
            AudioListener.pause = true; // 暂停音频
        }
        else
        {
            Debug.LogWarning("无法显示游戏结束界面，使用备用方案");
            ReloadScene(); // 备用方案：回到主界面
        }
    }
    
    // 重新开始游戏
    public static void RestartGame()
    {
        // 强制恢复所有设置
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        Score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // 返回主界面
    public static void ReturnToHome()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        Score = 0;
        SceneManager.LoadScene("Home");
    }
    
    public static void ReloadScene()
    {
        // 先恢复时间设置
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        Default.GetComponent<AudioSource>().Stop();
        Default.StartCoroutine(DoReloadScene());
    }

    private static IEnumerator DoReloadScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Home");
    }

    public static void AddScore(int score)
    {
        Score += score;
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
        }
        UpdateScoreText();
    }

    static void UpdateScoreText()
    {
        if (Default != null && Default.mScoreText != null)
        {
            Default.mScoreText.text = $"Score: {Score}\nHigh Score: {HighScore}";
        }
    }
}