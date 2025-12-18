using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Handles player movement, shooting, lives, and power-up effects.

    // Movement Settings
    [SerializeField] float speed = 2.5f;
    [SerializeField] float rightBoundary = 11.5f;
    [SerializeField] float leftBoundary = -11.2f;
    [SerializeField] float upperBoundary = 6.5f;
    [SerializeField] float lowerBoundary = -4.5f;

    // Shooting Settings
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject tripleShotPrefab;
    [SerializeField] private GameObject fiveShotPrefab;
    [SerializeField] float startingFire = 1.2f;

    public bool tripleShotActive = false;
    public bool fiveShotActive = false;
    public float fireRate = 0.3f;
    private float lastFiredTime = -0.3f;

    // Shield Settings
    [SerializeField] private GameObject shieldObject;
    private bool shieldActive = false;

    // Player Stats
    public int startingLives = 3;
    public int currentLives;
    private int score = 0;

    // Audio Settings
    public AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip powerUpSound;
    public AudioClip damageSound;

    // External References
    public CameraShake cameraShake;
    public GameManager gameManager;
    public SpawnManager spawnManager;
    private Coroutine fiveShotCoroutine; // Keep track of the coroutine

    private void Start()
    {
        transform.position = Vector3.zero;

        spawnManager = FindObjectOfType<SpawnManager>();
        gameManager = FindObjectOfType<GameManager>();

        if (spawnManager == null)
        {
            Debug.LogError("SPAWN MANAGER IS NULL");
        }

        currentLives = startingLives;
        UpdateUI();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver) return; // Stop if game is over

        PlayerMovement();
        PlayerFire();
    }

    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // Clamp the player's position within the boundaries

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, leftBoundary, rightBoundary),
            Mathf.Clamp(transform.position.y, lowerBoundary, upperBoundary),
            0
        );
    }

    private void PlayerFire()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > lastFiredTime + fireRate)
        {
            FireLaser();
            lastFiredTime = Time.time;
        }
    }

    private void FireLaser()
    {
        GameObject projectilePrefab;

        if (tripleShotActive)
        {
            projectilePrefab = tripleShotPrefab;
        }
        else if (fiveShotActive)
        {
            projectilePrefab = fiveShotPrefab;
        }
        else
        {
            projectilePrefab = laserPrefab;
        }

        Vector3 position = new Vector3(0, startingFire, 0);
        Instantiate(projectilePrefab, transform.position + position, Quaternion.identity);
        audioSource.PlayOneShot(shootingSound);
    }

    public void Damage()
    {
        if (shieldActive)
        {
            DeactivateShield();
            return;
        }

        currentLives--;
        audioSource.PlayOneShot(damageSound);
        cameraShake.Shake();
        UpdateUI();

        if (currentLives <= 0)
        {
            Die();
        }
    }

    private void DeactivateShield()
    {
        shieldObject.SetActive(false);
        shieldActive = false;
    }

    private void Die()
    {
        Destroy(gameObject);
        spawnManager.PlayerDied();

        if (gameManager != null)
        {
            gameManager.TriggerGameOver();
            StopAllCoroutines();
        }
    }

    public void ActivateTripleShot()
    {
        tripleShotActive = true;
        audioSource.PlayOneShot(powerUpSound);
        StartCoroutine(DeactivateTripleShot());
    }

    private IEnumerator DeactivateTripleShot()
    {
        yield return new WaitForSeconds(30f);
        tripleShotActive = false;
    }

    public void ActivateFiveShot()
    {
        Debug.Log("Five Shot Activated!");
        tripleShotActive = false; // Deactivate triple shot
        fiveShotActive = true; // Activate five shot
        audioSource.PlayOneShot(powerUpSound);

        // Reset the five-shot coroutine 
        ResetFiveShotCoroutine();
    }

    private IEnumerator DeactivateFiveShot()
    {
        yield return new WaitForSeconds(30f);
        fiveShotActive = false;
    }

    public void ResetFiveShotCoroutine()
    {
        // Stop the existing coroutine if its running
        if (fiveShotCoroutine != null)
        {
            StopCoroutine(fiveShotCoroutine);
            fiveShotCoroutine = null; // Reset the coroutine 
        }

        // Start a new coroutine for the fiveShot power-up duration
        fiveShotCoroutine = StartCoroutine(DeactivateFiveShot());
    }

    public void ActivateShield()
    {
        shieldActive = true;
        shieldObject.SetActive(true);
        audioSource.PlayOneShot(powerUpSound);
        StartCoroutine(ShieldPowerUpCoroutine());
    }

    private IEnumerator ShieldPowerUpCoroutine()
    {
        yield return new WaitForSeconds(5f);
        DeactivateShield();
    }

    public void AddScore()
    {
        score++;
        gameManager?.UpdateScore(score); // Null conditional operator
    }

    public void UpdateUI()
    {
        for (int i = 0; i < gameManager.livesImage.Length; i++)
        {
            gameManager.livesImage[i].enabled = i < currentLives; // Show / hide lives
        }
    }
}

