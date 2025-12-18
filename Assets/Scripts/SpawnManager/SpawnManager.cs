using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Enemy and PowerUp Settings
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private float timeBetweenWaves = 2f;
    private int currentWave = 0;

    // Bomb and Spaceship Settings
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject spaceshipPrefab;
    public int rows = 3;
    public int columns = 4;
    public float spacing = 3f;
    public float yOffset = 10.0f;
    public float speed = 4f;
    public float spawnInterval = 1.0f;

    // Planet Settings
    [SerializeField] private GameObject[] planetPrefabs;
    private List<GameObject> activePlanets = new List<GameObject>();
    public float planetSpeed = 0.5f;

    // Game and Object Pooling References
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PoolManager poolManager;

    private bool isPlayerAlive = true;
    private float timer = 0f;
    private int[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        // Start coroutines for spawning
        StartCoroutine(SpawnRoutinePowerUp());
        StartCoroutine(SpawnBombRoutine());
        StartCoroutine(SpawnRoutineEnemy());
        StartCoroutine(SpawnRoutinePlanet());

        // If gameManager is null, try to find it in the scene
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        GenerateRandomGrid();
    }

    void Update()
    {
        if (GameManager.instance.isGameOver) return; // Stop if game is over

        // Move enemies down and destroy them if they go out of bounds
        MoveEnemiesDown();

        // Spawn spaceships at defined intervals
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnSpaceships();
            timer = 0f;
        }

        // Move active planets down and remove them if they go out of bounds
        MoveActivePlanetsDown();
    }

    private void MoveEnemiesDown()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.transform.Translate(speed * Time.deltaTime * Vector3.down);
                // Check if the enemy's Y position is lower than -8
                if (enemy.transform.position.y < -8f)
                {
                    Destroy(enemy);
                }
            }
        }
    }

    private void MoveActivePlanetsDown()
    {
        for (int i = activePlanets.Count - 1; i >= 0; i--)
        {
            GameObject planet = activePlanets[i];
            if (planet != null)
            {
                planet.transform.Translate(planetSpeed * Time.deltaTime * Vector3.down);
                if (planet.transform.position.y < -15f)
                {
                    Destroy(planet);
                    activePlanets.RemoveAt(i); // Remove the planet from the list
                }
            }
        }
    }

    IEnumerator SpawnRoutineEnemy()
    {
        while (isPlayerAlive)
        {
            int minEnemies = currentWave + 1; // Increase the number of enemies as waves increase
            int enemiesToSpawn = Random.Range(minEnemies, minEnemies + 2); // Random number of enemies to spawn

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Vector3 enemyPos = new Vector3(Random.Range(-9f, 9f), 7, 0);
                Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves + 7f); //  wait between waves
        }
    }

    IEnumerator SpawnRoutinePlanet()
    {
        while (isPlayerAlive)
        {
            Vector3 planetPosition = new Vector3(Random.Range(-10f, 10f), 14, 0);
            int randomPlanetIndex = Random.Range(0, planetPrefabs.Length);

            GameObject newPlanet = Instantiate(planetPrefabs[randomPlanetIndex], planetPosition, Quaternion.identity);
            activePlanets.Add(newPlanet);

            yield return new WaitForSeconds(Random.Range(30, 60));
        }
    }

    IEnumerator SpawnRoutinePowerUp()
    {
        while (isPlayerAlive)
        {
            Vector3 powerUpPos = new Vector3(Random.Range(-9f, 9f), 7, 0);
            int randomPowerUpIndex = Random.Range(0, powerUps.Length);
            Instantiate(powerUps[randomPowerUpIndex], powerUpPos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(10, 30));
        }
    }

    private IEnumerator SpawnBombRoutine()
    {
        while (isPlayerAlive)
        {
            Vector3 bombPos = new Vector3(Random.Range(-9f, 9f), Random.Range(1f, 6f), 0);
            GameObject bombObject = poolManager.GetObjectFromPool();
            if (bombObject != null)
            {
                bombObject.transform.position = bombPos;
                BombScale bombScale = bombObject.GetComponent<BombScale>();
                if (bombScale != null)
                {
                    bombScale.AssignPoolManager(poolManager);
                    bombScale.AssignGameManager(gameManager);
                    bombScale.SetOriginalPosition(bombPos);
                }
            }
            yield return new WaitForSeconds(15f);
        }
    }

    public void PlayerDied()
    {
        isPlayerAlive = false;
    }

    private void GenerateRandomGrid()
    {
        grid = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j] = Random.Range(0, 2); // Randomly assign 0 or 1
            }
        }
    }

    private void SpawnSpaceships()
    {
        // Calculate the total width and height of the grid
        float totalWidth = (columns - 1) * spacing;
        float totalHeight = (rows - 1) * spacing;

        // Get the screen's width and height in world coordinates
        float screenWidth = Camera.main.aspect * Camera.main.orthographicSize * 2;
        float screenHeight = Camera.main.orthographicSize * 2;

        // Calculate the range for spawning spaceships in the x-axis
        float minX = -screenWidth / 2 + totalWidth / 2;
        float maxX = screenWidth / 2 - totalWidth / 2;

        // Calculate the starting position randomly within the calculated range
        float startX = Random.Range(minX, maxX);
        float startY = yOffset + totalHeight / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] == 1)
                {
                    Vector3 position = new(startX + j, startY + i, 0);
                    Instantiate(spaceshipPrefab, position, Quaternion.identity);
                }
            }
        }

        GenerateRandomGrid(); // Generate a new random grid for the next wave
    }
}

