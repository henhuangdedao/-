
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class HomeUI : MonoBehaviour
{
    void Start()
    {
        // 查找开始按钮
        Transform btnTransform = transform.Find("BtnStart");
        if (btnTransform == null)
        {
            Debug.LogError("找不到BtnStart，尝试其他查找方式...");
            btnTransform = GameObject.Find("BtnStart").transform;
        }
        
        if (btnTransform != null)
        {
            Button btnStart = btnTransform.GetComponent<Button>();
            if (btnStart != null)
            {
                btnStart.onClick.AddListener(OnStartButtonClick);
                Debug.Log("开始按钮事件绑定成功！");
            }
        }
        void OnStartButtonClick()
        {
            Debug.Log("开始游戏！");
    
            // 添加淡出效果
            StartCoroutine(FadeOutAndLoadGame());
        }

        IEnumerator FadeOutAndLoadGame()
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
    
            float fadeTime = 1.0f; // 淡出时间
            float timer = 0f;
    
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
                yield return null;
            }
    
            canvasGroup.alpha = 0f;
            SceneManager.LoadScene("Game");
        }
    }
    
   
}