using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShaderBackgroundManager : MonoBehaviour
{
    [Header("Shader材质")]
    public Material transitionMaterial;
    
    [Header("背景纹理")]
    public Texture2D defaultBackground;
    public Texture2D[] ufoBackgrounds = new Texture2D[4];
    
    [Header("过渡设置")]
    [Range(0.5f, 5f)] public float transitionTime = 2f;
    
    private bool isTransitioning = false;
    private Coroutine resetCoroutine;
    
    void Start()
    {
        Debug.Log("=== ShaderBackgroundManager启动 ===");
        
        // 获取Quad的Renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log($"✅ 找到Renderer: {renderer.name}");
            
            // 使用material（不是sharedMaterial）
            transitionMaterial = renderer.material;
            
            if (transitionMaterial != null)
            {
                Debug.Log($"✅ 获取材质: {transitionMaterial.name}");
                Debug.Log($"✅ Shader: {transitionMaterial.shader.name}");
                
                // 检查并设置Shader
                CheckAndFixShader();
                
                // 设置渲染顺序
                renderer.sortingLayerName = "Background";
                renderer.sortingOrder = -100;
                
                // 设置默认纹理
                if (defaultBackground != null)
                {
                    transitionMaterial.mainTexture = defaultBackground;
                    transitionMaterial.color = Color.white;
                    Debug.Log($"✅ 设置默认纹理: {defaultBackground.name}");
                }
                else
                {
                    Debug.LogError("❌ defaultBackground为null");
                }
            }
            else
            {
                Debug.LogError("❌ 无法获取材质");
            }
        }
        else
        {
            Debug.LogError("❌ 找不到Renderer组件");
        }
    }
    
    // 检查并修复Shader
    void CheckAndFixShader()
    {
        if (transitionMaterial == null) return;
        
        string shaderName = transitionMaterial.shader.name;
        Debug.Log($"当前Shader: {shaderName}");
        
        // 如果Shader不支持透明，换成Unlit/Transparent
        if (!shaderName.Contains("Transparent") && 
            !shaderName.Contains("UI") && 
            !shaderName.Contains("Sprite"))
        {
            Shader transparentShader = Shader.Find("Unlit/Transparent");
            if (transparentShader != null)
            {
                transitionMaterial.shader = transparentShader;
                Debug.Log($"✅ 切换到透明Shader: {transparentShader.name}");
            }
            else
            {
                Debug.LogError("❌ 找不到Unlit/Transparent Shader");
            }
        }
        else
        {
            Debug.Log($"✅ Shader已支持透明");
        }
    }
    
    public void SwitchToUFOBackground(int ufoType)
    {
        Debug.Log($"🎯 收到背景切换请求 - 类型: {ufoType}");
        
        if (isTransitioning)
        {
            Debug.LogWarning("⚠️ 正在过渡中，跳过");
            return;
        }
        
        if (ufoType < 0 || ufoType >= 4)
        {
            Debug.LogError($"❌ 无效UFO类型: {ufoType}");
            return;
        }
        
        if (ufoBackgrounds[ufoType] != null)
        {
            Debug.Log($"🔄 开始切换背景: {ufoBackgrounds[ufoType].name}");
            StartCoroutine(TransitionToTexture(ufoBackgrounds[ufoType]));
        }
        else
        {
            Debug.LogError($"❌ UFO背景{ufoType}为null");
        }
    }
    
    IEnumerator TransitionToTexture(Texture2D newTexture)
    {
        if (transitionMaterial == null || newTexture == null)
        {
            Debug.LogError("❌ 过渡失败: 材质或纹理为null");
            yield break;
        }
        
        isTransitioning = true;
        
        Debug.Log($"=== 开始背景过渡 ===");
        Debug.Log($"   从: {transitionMaterial.mainTexture?.name ?? "无"}");
        Debug.Log($"   到: {newTexture.name}");
        Debug.Log($"   当前Alpha: {transitionMaterial.color.a:F3}");
        
        // 保存原始颜色
        Color originalColor = transitionMaterial.color;
        
        // 1. 淡出当前纹理
        float timer = 0f;
        while (timer < transitionTime * 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / (transitionTime * 0.5f);
            
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            transitionMaterial.color = newColor;
            
            Debug.Log($"  淡出 - 进度: {t:F2}, Alpha: {newColor.a:F3}");
            yield return null;
        }
        
        // 2. 切换纹理
        transitionMaterial.mainTexture = newTexture;
        Debug.Log($"✅ 纹理已切换");
        
        // 3. 淡入新纹理
        timer = 0f;
        Color transparentColor = transitionMaterial.color;
        transparentColor.a = 0f;
        
        while (timer < transitionTime * 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / (transitionTime * 0.5f);
            
            Color newColor = transparentColor;
            newColor.a = Mathf.Lerp(0f, 1f, t);
            transitionMaterial.color = newColor;
            
            Debug.Log($"  淡入 - 进度: {t:F2}, Alpha: {newColor.a:F3}");
            yield return null;
        }
        
        // 4. 恢复完全不透明
        transitionMaterial.color = Color.white;
        
        isTransitioning = false;
        Debug.Log($"✅ 背景过渡完成");
    }
    
    public void ResetToDefault()
    {
        if (defaultBackground != null && !isTransitioning)
        {
            Debug.Log($"🔄 重置到默认背景: {defaultBackground.name}");
            StartCoroutine(TransitionToTexture(defaultBackground));
        }
        else if (isTransitioning)
        {
            Debug.LogWarning("⚠️ 正在过渡中，无法重置");
        }
    }
    
    public void ResetAfterDelay(float delay)
    {
        Debug.Log($"⏳ 设置延迟重置: {delay}秒后");
        
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        
        resetCoroutine = StartCoroutine(DelayedReset(delay));
    }
    
    IEnumerator DelayedReset(float delay)
    {
        Debug.Log($"⏳ 等待{delay}秒后重置...");
        yield return new WaitForSeconds(delay);
        Debug.Log($"⏳ 延迟结束，开始重置");
        ResetToDefault();
    }
    
    // 测试功能
    void Update()
    {
        // 按数字键测试
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("🎮 按1键测试");
            SwitchToUFOBackground(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("🎮 按2键测试");
            SwitchToUFOBackground(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("🎮 按3键测试");
            SwitchToUFOBackground(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("🎮 按4键测试");
            SwitchToUFOBackground(3);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("🎮 按R键重置");
            ResetToDefault();
        }
        
        // 显示当前材质状态
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (transitionMaterial != null)
            {
                Debug.Log($"📊 当前材质状态:");
                Debug.Log($"   纹理: {transitionMaterial.mainTexture?.name ?? "无"}");
                Debug.Log($"   透明度: {transitionMaterial.color.a:F3}");
                Debug.Log($"   Shader: {transitionMaterial.shader.name}");
            }
        }
    }
}