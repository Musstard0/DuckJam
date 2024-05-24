using System;
using DG.Tweening;
using DuckJam;
using DuckJam.Utilities;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float rotationSpeed = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector3 firePointInvertedPositionOffset;

    private Vector3 _defaultLocalPosition;
    private Vector3 _firePointDefaultLocalPosition;

    //private float _firePointPlayerPositionOffset;
    
    public Transform player;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;
    
    private PlayerModel _playerModel;
    
    private Sequence _sequence;

    private void Start()
    {
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        gunSpriteRenderer = GetComponent<SpriteRenderer>();
        _playerModel = GameModel.Get<PlayerModel>();
        
        _defaultLocalPosition = transform.localPosition;
        _firePointDefaultLocalPosition = firePoint.localPosition;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    private void Update()
    {
        // Convert mouse position to world position considering the perspective camera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xyPlane = new Plane(Vector3.forward, new Vector3(0, 0, player.position.z));
        if (xyPlane.Raycast(ray, out float distance))
        {
            var mousePos = ray.GetPoint(distance).XY();
            Vector2 direction = (mousePos - firePoint.position.XY()).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            
            // Rotate the gun towards the mouse position
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Flip the player and gun sprites based on the mouse position
            if (transform.eulerAngles.z is >= 270f or <= 90f)
            {
                gunSpriteRenderer.flipY = false;
                firePoint.localPosition = _firePointDefaultLocalPosition;
            }
            else
            {
                gunSpriteRenderer.flipY = true; // Assuming the gun sprite is horizontal by default
                firePoint.localPosition = _firePointDefaultLocalPosition + firePointInvertedPositionOffset;
            }
        }
        
        if(!_sequence.IsActive()) return;
        
        _sequence.timeScale = _playerModel.TimeScale;
    }

    [SerializeField] private Ease kickbackEase = Ease.OutQuad; 
    [SerializeField] private Ease kickbackResetEase = Ease.OutQuad;
    [SerializeField, Min(0f)] private float kickbackDuration = 0.1f;
    [SerializeField, Min(0f)] private float kickbackResetDuration = 0.1f;
    [SerializeField, Min(0f)] private float kickbackDistance = 0.1f;
    
    public void OnFire()
    {
        _sequence?.Complete();
        
        _sequence = DOTween.Sequence();
        _sequence.timeScale = _playerModel.TimeScale;
        
        var kickEndPosition = _defaultLocalPosition - transform.right * kickbackDistance;


        _sequence.Append
        (
            transform
                .DOLocalMove(kickEndPosition, kickbackDuration)
                .SetEase(kickbackEase)
        );
        
        _sequence.Append
        (
            transform
                .DOLocalMove(_defaultLocalPosition, kickbackResetDuration)
                .SetEase(kickbackResetEase)
        );

        _sequence.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 5f);
    }
}
