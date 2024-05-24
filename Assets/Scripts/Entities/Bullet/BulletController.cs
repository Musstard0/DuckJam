using System;
using DuckJam.Entities;
using DuckJam.Modules;
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
    public bool Exploded { get; set; }
    public ImpactFXColor ImpactFXColor { get; set; }
    
    public Vector2 Position2D => transform.position.XY();

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(Exploded) return;
        
        var collisionLayer = other.gameObject.layer;

        if (collisionLayer == LayerUtils.TerrainLayer)
        {
            Explode();
            return;
        }

        if (collisionLayer == LayerUtils.ProjectileLayer)
        {
            var otherBullet = other.GetComponent<BulletController>();
            if (otherBullet != null && otherBullet.TargetLayer != TargetLayer)
            {
                Explode();
                otherBullet.Explode();
                return;
            }
        }
        
        if(other.gameObject.layer != TargetLayer) return;
        
        var damageable = other.GetComponent<IDamageable>();
        if(damageable != null) damageable.TakeDamage(Damage, Direction);

        Explode();
    }


    private void Explode()
    {
        if(Exploded) return;
        Exploded = true;

        var frames = SpriteAnimationManager.Instance.ImpactFXSpriteArr[6].GetFramesForColor(ImpactFXColor);
        SpriteAnimationManager.Instance.CreateImpactAnimationEffect(frames, transform.position.XY(), 10f, TimeScale);
        DisposeAction.Invoke(this);
    }
}
