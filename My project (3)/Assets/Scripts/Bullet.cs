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
        transform.Translate(direction * Time.deltaTime * 10f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //获得石头组件
        Rock rock = other.GetComponent<Rock>();
        //如果是大石头则分裂
        if (rock.Big)
        {
            //设置本身为小石头
            rock.SmallSize();
            //克隆一个小石头
            Rock rock2 = Instantiate(rock);
            //同步位置
            rock2.transform.position = rock.transform.position;
           
           
        }
        else
        {
            //销毁石头
            Destroy(other.gameObject);   
        }
        
        //销毁子弹
        Destroy(gameObject);
    }
}

