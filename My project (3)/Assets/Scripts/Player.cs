using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utility;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 7f;
    private Rigidbody2D rb;
    public float fireRate = 0.5f;
    private float lastFireTime = 0f;
    private bool ignoreNextClick = false; // 新增：忽略下一次点击
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.None;
    }

    void Update()
    {
        ScreenHelper.RepeatScreen(transform, offsetX: 0, offsetY: 0);
        
        // 移动逻辑
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
        if (Input.GetKey(KeyCode.A)) direction += Vector2.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector2.right;
        
        rb.velocity = direction.normalized * speed;
        
        // 玩家朝向鼠标方向
        LookAtMouse();
        
        // 发射逻辑：添加UI检测和点击忽略
        if (Input.GetMouseButton(0) && !IsPointerOverUI() && !ignoreNextClick && Time.time - lastFireTime >= fireRate)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
        
        // 重置点击忽略标志
        if (ignoreNextClick && !Input.GetMouseButton(0))
        {
            ignoreNextClick = false;
        }
    }
    
    // 检测鼠标是否在UI上
    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    
    // 新增：忽略下一次点击的方法（从UIPause调用）
    public void IgnoreNextClick()
    {
        ignoreNextClick = true;
    }
    
    void LookAtMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 directionToMouse = (mousePosition - transform.position).normalized;
        
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    void FireBullet()
    {
        Transform bulletTemplate = transform.Find("Bullet");
        Transform bullet = Instantiate(bulletTemplate);
        bullet.position = bulletTemplate.position;
        
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 shootDirection = (mousePosition - transform.position).normalized;
        bulletComponent.direction = shootDirection;
        
        float bulletAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg - 90f;
        bullet.rotation = Quaternion.AngleAxis(bulletAngle, Vector3.forward);
        
        bullet.gameObject.SetActive(true);
        transform.Find("SfxShoot").GetComponent<AudioSource>().Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Rock"))
        {
            var sfx = Instantiate(transform.Find("SfxLose"), null);
            sfx.transform.position = transform.position;
            sfx.GetComponent<AudioSource>().Play();
            gameObject.SetActive(false);
        
            // 改为显示游戏结束界面
            Game.ShowGameOver();
        }
    }
}


        
