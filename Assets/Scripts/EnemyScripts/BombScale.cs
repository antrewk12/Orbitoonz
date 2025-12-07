using UnityEngine;
using System.Collections;

public class BombScale : MonoBehaviour
{
    // Scaling Settings
    [SerializeField] private float scaleFactor = 1.1f;  // Scaling factor
    [SerializeField] private float scaleInterval = 1f;  // Time between scaling
    [SerializeField] private float minScaleFactor = 0.2f;
    [SerializeField] private float maxScaleFactor = 0.4f;

    // Rotation Settings
    [SerializeField] private float rotationSpeed = 350f; // Bomb rotation speed

    // Health Settings
    [SerializeField] private float initialBombHealth = 150f;
    [SerializeField] private float laserDamage = 5f;

    // Particle and Explosion Effects
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private GameObject explosionBigPrefab;

    // Camera Shake Settings
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.01f;

    // Game State
    private float bombHealth;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;
    private GameManager gameManager;
    private PoolManager poolManager;

    // Audio Settings
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip gameOverExplosion;

    private void OnEnable()
    {
        SetRandomInitialScale();
        ResetBomb();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.localPosition = originalPosition;  // Ensure position is reset
        transform.localScale = Vector3.one;  // Reset scale
    }

    public void ResetBomb()
    {
        bombHealth = initialBombHealth;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    private void SetRandomInitialScale()
    {
        float randomScale = Random.Range(minScaleFactor, maxScaleFactor);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver) return; // Stop if game is over

        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);  // Rotate the bomb

        if (ShouldScale())
        {
            ScaleBomb();
        }
    }

    private bool ShouldScale()
    {
        return Time.time % scaleInterval < Time.deltaTime; // Check if it's time to scale
    }

    private void ScaleBomb()
    {
        transform.localScale *= scaleFactor;

        if (transform.localScale.x > 1.5f)
        {
            gameManager.TriggerGameOver();
            StartCoroutine(HandleBombsDeath());
        }
    }

    public void AssignGameManager(GameManager managerGame)
    {
        gameManager = managerGame;
    }

    public void AssignPoolManager(PoolManager manager)
    {
        poolManager = manager;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            HandleLaserCollision(other);
        }
        else if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other);
        }
    }

    private void HandleLaserCollision(Collider2D laser)
    {
        bombHealth -= laserDamage;
        Destroy(laser.gameObject); // Destroy the laser

        if (bombHealth <= 0)
        {
            StartCoroutine(HandleBombDestruction());
        }
        else
        {
            Shake();
        }
    }

    private void HandlePlayerCollision(Collider2D playerCollider)
    {
        PlayerController playerController = playerCollider.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.Damage();
            StartCoroutine(HandleBombDestruction());
        }
    }

    private IEnumerator HandleBombDestruction()
    {
        GameObject particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        yield return PlayExplosionSound(explosionSound);

        Destroy(particles, particles.GetComponent<ParticleSystem>().main.duration);

        ReturnToPool();
    }

    private IEnumerator HandleBombsDeath()
    {
        GameObject particles = Instantiate(explosionBigPrefab, transform.position, Quaternion.identity);
        yield return PlayExplosionSound(gameOverExplosion);
    }

    private IEnumerator PlayExplosionSound(AudioClip clip)
    {
        if (audioSource && clip != null)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }

    private void ReturnToPool()
    {
        if (poolManager != null)
        {
            poolManager.ReturnObjectToPool(gameObject);
        }
    }

    public void Shake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomPoint = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = new Vector3(randomPoint.x, randomPoint.y, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}

