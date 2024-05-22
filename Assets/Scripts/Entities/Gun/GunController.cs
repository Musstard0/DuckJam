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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Flip the player and gun sprites based on the mouse position
        if (mousePos.x < player.position.x)
        {
            playerSpriteRenderer.flipX = true;
            gunSpriteRenderer.flipY = true; // Assuming the gun sprite is horizontal by default
        }
        else
        {
            playerSpriteRenderer.flipX = false;
            gunSpriteRenderer.flipY = false;
        }
    }
}
