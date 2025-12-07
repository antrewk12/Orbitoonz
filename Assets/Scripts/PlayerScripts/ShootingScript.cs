using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public float speed = 5f;


    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);

        if (transform.position.y > 7f)
        {
            Debug.Log($"Destroying projectile: {gameObject.name}");

            if (transform.parent != null)
            {
                Debug.Log($"Destroying parent object: {transform.parent.name}");
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Avoid destroying projectiles if they hit the player
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Projectile collided with player, not destroying.");
            return;
        }

        Debug.Log($"Projectile hit: {other.gameObject.name}");
        Destroy(gameObject);
    }

}
