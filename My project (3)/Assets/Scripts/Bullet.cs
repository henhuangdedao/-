using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只销毁障碍物，避免误伤其他物体
        if (other.CompareTag("obstacle"))
        {
            Destroy(other.gameObject);  // 销毁石头
            Destroy(gameObject);         // 同时销毁子弹本身
        }
    }
}
