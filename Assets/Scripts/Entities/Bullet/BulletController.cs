using System;
using DuckJam.Entities;
using DuckJam.Utilities;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D Rigidbody2D { get; private set; }
    
    public Action<BulletController> DisposeAction;
    
    public Vector2 Direction { get; set; }
    public int TargetLayer { get; set; } = -1;
    public float Damage { get; set; }
    public float Speed { get; set; }
    public float TimeScale { get; set; } = 1f;
    
    public Vector2 Position2D => transform.position.XY();

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(TargetLayer < 0) return;
        if(other.gameObject.layer != TargetLayer) return;
        
        var damageable = other.GetComponent<IDamageable>();
        if(damageable != null) damageable.TakeDamage(Damage);
        
        DisposeAction.Invoke(this);
    }
}
