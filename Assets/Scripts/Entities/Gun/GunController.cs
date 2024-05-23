using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float rotationSpeed = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector3 firePointInvertedPositionOffset;
    
    private Vector3 _firePointDefaultLocalPosition;
    
    public Transform player;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;

    void Start()
    {
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        gunSpriteRenderer = GetComponent<SpriteRenderer>();
        
        _firePointDefaultLocalPosition = firePoint.localPosition;
    }

    void Update()
    {
        // Convert mouse position to world position considering the perspective camera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xyPlane = new Plane(Vector3.forward, new Vector3(0, 0, player.position.z));
        if (xyPlane.Raycast(ray, out float distance))
        {
            Vector3 mousePos = ray.GetPoint(distance);
            Vector2 direction = mousePos - player.position;
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
    }
}
