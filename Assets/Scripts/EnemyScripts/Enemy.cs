using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float minSpeed = 2f;
    [SerializeField] float maxSpeed = 4f;
    [SerializeField] float moveSpeed;
    private PlayerController playerController;
    public GameObject deathParticlesPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime * Vector3.down);

        if(transform.position.y < -6f)
        {
            transform.position = new Vector3(Random.Range(-9f, 9f), 7f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

            if(playerController.tripleShotActive == true || playerController.fiveShotActive == true)
            {
                Destroy(other.transform.parent.gameObject);
            }

           if(playerController != null)
            {
                playerController.AddScore();
            }

            // Instantiate death particles
            GameObject particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);

            // Get the duration of the particle system
            float particleDuration = particles.GetComponent<ParticleSystem>().main.duration;

            // Destroy the particle system after its duration
            Destroy(particles, particleDuration);
        }

        if (other.CompareTag("Player"))
        {
            //PlayerController playerController = other.transform.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.Damage();
                Destroy(this.gameObject);
            }

            // Instantiate death particles
            GameObject particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            // Get the duration of the particle system
            float particleDuration = particles.GetComponent<ParticleSystem>().main.duration;
            // Destroy the particle system after its duration
            Destroy(particles, particleDuration);

        }
    }

    


}
