using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    private Transform target; // 添加目标变量
    public float rotationSpeed = 200f; // 添加旋转速度

    // Start is called before the first frame update
    void Start()
    {
        // 添加：发射时寻找最近的目标
        FindNearestTarget();
        
        // 添加：如果没有目标，5秒后自毁
        if (target == null)
            Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        // 修改移动逻辑：添加追踪功能
        if (target == null)
        {
            // 如果目标丢失，重新寻找
            FindNearestTarget();
            if (target == null)
            {
                // 没有目标时直线飞行（保持原有逻辑）
                transform.Translate(direction * Time.deltaTime * 10f);
                return;
            }
        }
        
        // 有目标时进行追踪
        // 计算指向目标的方向
        Vector2 targetDirection = (target.position - transform.position).normalized;
        
        // 旋转朝向目标
        float rotateAmount = Vector3.Cross(targetDirection, (Vector2)transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * rotationSpeed * Time.deltaTime);
        
        // 朝当前方向前进（现在方向会追踪目标）
        transform.Translate(Vector2.up * Time.deltaTime * 10f, Space.Self);
    }

    // 添加：寻找最近目标的方法
    void FindNearestTarget()
    {
        Rock[] rocks = FindObjectsOfType<Rock>();
        if (rocks.Length == 0) return;
        
        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Rock rock in rocks)
        {
            float distance = Vector2.Distance(transform.position, rock.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = rock.transform;
            }
        }
        
        target = nearestTarget;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Rock"))
        {
            //获得石头组件
            Rock rock = other.GetComponent<Rock>();
            rock.PlaySfxRockDestroy();

            Game.AddScore(50);
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
            else if (FindObjectsByType<Rock>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length < 12)
            {
                rock.BigSize();
                rock.transform.position = new Vector2(ScreenHelper.LeftBottomScreenPos.x - rock.SpriteSize.x * 0.5f,
                    rock.transform.position.y);

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
}
