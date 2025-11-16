using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 7f;
    private Rigidbody2D rb; // 声明

    void Start()
    {
        // 初始化：获取当前物体上的Rigidbody2D组件
        rb = GetComponent<Rigidbody2D>();
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

        // 使用已初始化的rb
        rb.velocity = direction.normalized * speed;
        if (Input.GetMouseButtonDown(0))
        {
            //拿到 Bullet
            Transform bulletTemplate = transform.Find("Bullet");
            //通过 Bullet 克隆个新的 bullet
            Transform bullet = Instantiate(bulletTemplate);
            //设置位置
            bullet.position = bulletTemplate.position;
            //获得 Bullet 脚本(组件)
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            //投置子弹的发射方向
            bulletComponent.direction = transform.up;
            //显示子弹
            bullet.gameObject.SetActive(true);
            //播放开枪音频
            transform.Find("SfxShoot").GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Rock"))
        {
            var sfx = Instantiate(transform.Find("SfxLose"), null);
            sfx.transform.position = transform.position;
            sfx.GetComponent<AudioSource>().Play();

            gameObject.SetActive(false);

            Game.ReloadScene();

        }



    }
}



        
