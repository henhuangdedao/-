using UnityEngine;
using Utility;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10f;

    void Start()
    {
        Destroy(gameObject, 5f);
    }
    
    void Update()
    {
        transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("UFO"))
        {
            Destroy(other.gameObject);
            Game.AddScore(100);
            Destroy(gameObject);
        }

        if (other.gameObject.name.StartsWith("Rock"))
        {
            Rock rock = other.GetComponent<Rock>();
            rock.PlaySfxRockDestroy();
            Game.AddScore(50);
            Destroy(rock.gameObject);
            Destroy(gameObject);
        }
    }
}