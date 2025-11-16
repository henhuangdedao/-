using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Rock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //0~360 随机一个角度
        float angle = Random.Range(0f, 360f);
        //将角度转换成向量
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        //将向量设置给刚体速度
        GetComponent<Rigidbody2D>().velocity = direction;
        //设置旋转速度
        GetComponent<Rigidbody2D>().angularVelocity = 60f;

    }

    private void Awake()
    {
      mSpriteSize = transform.Find("SpriteBig").GetComponent<SpriteRenderer>().size;
    }
    public bool Big = true;
    private Vector2 mSpriteSize;
    public Vector2 SpriteSize=> mSpriteSize;

    public void PlaySfxRockDestroy()
    {
        var sfx = Instantiate(transform.Find("SfxRockDestroy"),null);
              sfx.transform.position = transform.position;
              sfx.GetComponent<AudioSource>().Play();
    }
    public void BigSize()
    {
        Big = true;
        GetComponent<CircleCollider2D>().radius = 1.41f;
        transform.Find("SpriteBig").gameObject.SetActive(true);
        transform.Find("SpriteSmall").gameObject.SetActive(false);
        mSpriteSize=transform.Find("SpriteBig").GetComponent<SpriteRenderer>().size;
    }

    public void SmallSize()
    {
        Big = false;
        GetComponent<CircleCollider2D>().radius = 0.7f;
        transform.Find("SpriteBig").gameObject.SetActive(false);
        transform.Find("SpriteSmall").gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        var offsetX = mSpriteSize.x;
        var offsetY = mSpriteSize.y;
       ScreenHelper.RepeatScreen(transform,offsetX,offsetY);
    }
    
}
