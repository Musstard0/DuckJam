using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float LifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collision with enemies
        //if (other.CompareTag("Enemy"))
        //{
        //    // Deal damage to enemy
        //    Destroy(gameObject);
        //}
    }
}
