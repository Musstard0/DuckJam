using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float LifeTime = 2f;

    public int TargetLayer { get; set; } = -1;
    
    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(TargetLayer < 0) return;
        if(other.gameObject.layer != TargetLayer) return;
        
        Destroy(gameObject);
        
        // Handle collision with enemies
        //if (other.CompareTag("Enemy"))
        //{
        //    // Deal damage to enemy
        //    Destroy(gameObject);
        //}
    }
}
