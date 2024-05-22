using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform player;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;

    void Start()
    {
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        gunSpriteRenderer = GetComponent<SpriteRenderer>();
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
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Flip the player and gun sprites based on the mouse position
            if (mousePos.x < player.position.x)
            {
                gunSpriteRenderer.flipY = true; // Assuming the gun sprite is horizontal by default
            }
            else
            {
                gunSpriteRenderer.flipY = false;
            }
        }
    }
}
