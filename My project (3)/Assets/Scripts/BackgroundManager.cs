using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("背景设置")]
    public Renderer backgroundQuad;
    public Texture2D defaultBackground;
    public Texture2D[] ufoBackgrounds = new Texture2D[4];  // 4个UFO背景
    
    [Header("过渡效果")]
    public float transitionTime = 1f;
    
    private Material quadMaterial;
    private bool isTransitioning = false;
    private Coroutine resetCoroutine;
    
    void Start()
    {
        if (backgroundQuad != null)
        {
            quadMaterial = backgroundQuad.material;
            
            if (defaultBackground != null)
            {
                quadMaterial.mainTexture = defaultBackground;
            }
        }
    }
    
    // 🆕 切换到UFO背景
    public void SwitchToUFOBackground(int ufoType)
    {
        if (isTransitioning || ufoType < 0 || ufoType >= 4) return;
        
        if (ufoBackgrounds[ufoType] != null)
        {
            StartCoroutine(TransitionToTexture(ufoBackgrounds[ufoType]));
        }
    }
    
    // 🆕 重置为默认背景
    public void ResetToDefault()
    {
        if (defaultBackground != null && !isTransitioning)
        {
            StartCoroutine(TransitionToTexture(defaultBackground));
        }
    }
    
    IEnumerator TransitionToTexture(Texture2D newTexture)
    {
        isTransitioning = true;
        
        // 淡出
        float timer = 0f;
        while (timer < transitionTime * 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / (transitionTime * 0.5f);
            
            Color color = quadMaterial.color;
            color.a = Mathf.Lerp(1f, 0.3f, t);
            quadMaterial.color = color;
            
            yield return null;
        }
        
        // 切换纹理
        quadMaterial.mainTexture = newTexture;
        
        // 淡入
        timer = 0f;
        while (timer < transitionTime * 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / (transitionTime * 0.5f);
            
            Color color = quadMaterial.color;
            color.a = Mathf.Lerp(0.3f, 1f, t);
            quadMaterial.color = color;
            
            yield return null;
        }
        
        quadMaterial.color = Color.white;
        isTransitioning = false;
    }
    
    // 🆕 延迟重置背景
    public void ResetAfterDelay(float delay)
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(DelayedReset(delay));
    }
    
    IEnumerator DelayedReset(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetToDefault();
    }
    
    void Update()
    {
        // 测试：按数字键1-4切换，按R重置
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToUFOBackground(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToUFOBackground(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToUFOBackground(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchToUFOBackground(3);
        if (Input.GetKeyDown(KeyCode.R)) ResetToDefault();
    }
}