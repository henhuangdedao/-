using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameMouse : MonoBehaviour
{
    [Header("鼠标设置")]
    public Sprite mouseSprite;      // 拖入你的鼠标图片
    public float mouseScale = 0.3f;
    
    [Header("UI检测")]
    public GameObject uiPause;
    public GameObject uiGameOver;
    
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool isUIactive = false;
    
    void Start()
    {
        Cursor.visible = false;
        mainCamera = Camera.main;
        
        SetupMouseAppearance();
    }
    
    void SetupMouseAppearance()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        
        if (mouseSprite != null)
        {
            spriteRenderer.sprite = mouseSprite;
        }
        
        transform.localScale = Vector3.one * mouseScale;
        spriteRenderer.sortingOrder = 9999;
    }
    
    void Update()
    {
        CheckUIStatus();
        
        if (!isUIactive)
        {
            UpdateMousePosition();
        }
    }
    
    void CheckUIStatus()
    {
        bool wasUIactive = isUIactive;
        
        bool pauseActive = (uiPause != null && uiPause.activeInHierarchy);
        bool gameOverActive = (uiGameOver != null && uiGameOver.activeInHierarchy);
        
        isUIactive = pauseActive || gameOverActive;
        
        if (wasUIactive != isUIactive)
        {
            OnUIStatusChanged();
        }
    }
    
    void OnUIStatusChanged()
    {
        if (isUIactive)
        {
            Cursor.visible = true;
            SetMouseVisible(false);
        }
        else
        {
            Cursor.visible = false;
            SetMouseVisible(true);
        }
    }
    
    void UpdateMousePosition()
    {
        if (mainCamera == null) return;
        
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
    }
    
    public void SetMouseVisible(bool visible)
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = visible;
    }
    
    public void SetUIActive(bool uiActive)
    {
        isUIactive = uiActive;
        OnUIStatusChanged();
    }
    
    public Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector3.zero;
        
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f;
        return mainCamera.ScreenToWorldPoint(screenPos);
    }
    
    void OnDestroy()
    {
        Cursor.visible = true;
    }
}