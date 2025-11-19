using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10f; // 保持原飞行速度不变

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // 直接直线飞行，删除所有自瞄逻辑
        transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Rock"))
        {
            Rock rock = other.GetComponent<Rock>();
            rock.PlaySfxRockDestroy();
            Game.AddScore(50);
            
            if (rock.Big)
            {
                rock.SmallSize();
                Rock rock2 = Instantiate(rock);
                rock2.transform.position = rock.transform.position;
            }
            else if (FindObjectsByType<Rock>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length < 12)
            {
                rock.BigSize();
                rock.transform.position = new Vector2(ScreenHelper.LeftBottomScreenPos.x - rock.SpriteSize.x * 0.5f,
                    rock.transform.position.y);
            }
            else
            {
                Destroy(other.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
