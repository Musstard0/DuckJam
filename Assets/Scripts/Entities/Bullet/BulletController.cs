using System;
using DuckJam.Entities;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float _damage;
    
    //public float LifeTime = 2f;

    public Action<BulletController> DisposeAction;
    
    public int TargetLayer { get; set; } = -1;

    public float Damage
    {
        get => _damage;
        set => _damage = Mathf.Max(value, 0f);
    }

    private void Start()
    {
        //Destroy(gameObject, LifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(TargetLayer < 0) return;
        if(other.gameObject.layer != TargetLayer) return;
        
        var damageable = other.GetComponent<IDamageable>();
        if(damageable != null) damageable.TakeDamage(Damage);
        
        DisposeAction?.Invoke(this);
        
        //Destroy(gameObject);
        
        // Handle collision with enemies
        //if (other.CompareTag("Enemy"))
        //{
        //    // Deal damage to enemy
        //    Destroy(gameObject);
        //}
    }
}
