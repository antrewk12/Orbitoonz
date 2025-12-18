using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Manages overall game state, UI, score, audio, and scene restarting.

    // Singleton Instance
    public static GameManager instance;

    // Audio Settings
    [Header("Audio Settings")]
    [SerializeField] private AudioSource backgroundMusic;       // Main audio source for music
    [SerializeField] private AudioClip gamePlayMusic;           // The gameplay background music
    [SerializeField] private AudioClip gameOverMusic;           // The music that plays when the player dies or the UI is shown
    [SerializeField] private AudioClip gameOverSound;

    // Game State
    [Header("Game State")]
    public bool isGameOver = false;

    // UI Elements
    [Header("UI Elements")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text restartText;
    public Image[] livesImage;

    // Player Reference
    private PlayerController playerController;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
        UpdateBackgroundMusic(gamePlayMusic);
        scoreText.text = "Score: ";
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void UpdateBackgroundMusic(AudioClip musicClip)
    {
        if (backgroundMusic != null && musicClip != null)
        {
            backgroundMusic.clip = musicClip;
            backgroundMusic.loop = true;
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.Play(); // Play the music directly if not already playing
            }
        }
    }

    // Call this when the player dies or switches to the UI
    public void PlayGameOverMusic()
    {
        UpdateBackgroundMusic(gameOverMusic);  // Switch to the game over music
        isGameOver = true;
    }

    // Call this when the gameplay starts again 
    public void PlayGamePlayMusic()
    {
        UpdateBackgroundMusic(gamePlayMusic);  // Switch back to the gameplay music
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();

        // Press 'R' to restart the game if it's over
        if (Input.GetKeyDown(KeyCode.R) && isGameOver)
        {
            RestartGame();
        }
    }

    private void CheckGameOver()
    {
        if (playerController != null && playerController.currentLives <= 0)
        {
            TriggerGameOver();
        }
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        DisplayGameOver();
        PlayGameOverMusic();
    }

    public void DisplayGameOver()
    {
        // Display the game over text and restart instructions
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
    }

    // Method to restart the game
    public void RestartGame()
    {
        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Reload the active scene
        PlayGamePlayMusic(); // Ensure the gameplay music starts when restarting
        isGameOver = false;
    }
}
