using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    // Controls laser projectile movement and collision.

    [SerializeField] float speed = 5f;


    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);

        if (transform.position.y > 7f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // destroying projectiles if they hit the player

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }

        Destroy(gameObject);
    }

}
