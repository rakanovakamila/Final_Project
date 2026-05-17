using System;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;    
    public float thrustForce = 1f;
    private int highScore = 0;
    
    public GameObject boosterFlame;
    Rigidbody2D rb;
    public UIDocument uiDocument;
    private Label scoreText;
    private Label highScoreText;
    public GameObject explosionEffect;
    private Button restartButton;
    private Button menuButton;

    private VisualElement pauseMenuContainer;
    private Button returnButton;
    private Button pauseRestartButton;

    private bool gameStarted = false;
    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");

        pauseMenuContainer = uiDocument.rootVisualElement.Q<VisualElement>("PauseMenuContainer");
        returnButton = uiDocument.rootVisualElement.Q<Button>("ReturnButton");
        pauseRestartButton = uiDocument.rootVisualElement.Q<Button>("PauseRestartButton");
        menuButton = uiDocument.rootVisualElement.Q<Button>("MenuButton");
        if (restartButton != null) restartButton.style.display = DisplayStyle.None;
        if (highScoreText != null) highScoreText.style.display = DisplayStyle.None;
        if (pauseMenuContainer != null) pauseMenuContainer.style.display = DisplayStyle.None;
        if (menuButton != null) menuButton.style.display = DisplayStyle.None;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        restartButton.clicked += ReloadScene;
        pauseRestartButton.clicked += ReloadScene;
        returnButton.clicked += TogglePause;
        if(menuButton != null)
        {
            menuButton.clicked += ReturnToMainMenu;
        }
        if (MainMenuController.skipMenu)
        {
            StartGameLoop();
        }
        else
        {
            // First time booting up: make sure the score system is hidden behind the main menu
            if (scoreText != null) scoreText.style.display = DisplayStyle.None;
        }
    }

    public void StartGameLoop()
    {
        gameStarted = true;
        if (scoreText != null) scoreText.style.display = DisplayStyle.Flex;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        if (isPaused) return;
        // Score updater
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score; 

        // Booster Flame 
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }

        if (Mouse.current.leftButton.isPressed)
        {
            // calcu;ate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;

            //Move player in direction of mouse
            rb.AddForce(direction * thrustForce);

            
        }
    }

    void TogglePause()
    {
        if (!gameStarted) return;
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenuContainer.style.display = DisplayStyle.Flex;
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuContainer.style.display= DisplayStyle.None;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHighScore();
        Destroy(gameObject);
        Instantiate(explosionEffect, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;
        highScoreText.style.display = DisplayStyle.Flex;
        if (menuButton != null) menuButton.style.display = DisplayStyle.Flex;
    }

    private void HandleHighScore()
    {
        int currentScoreInt = Mathf.FloorToInt(score);
        if (currentScoreInt > highScore)
        {
            highScore = currentScoreInt;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        highScoreText.text = "High Score: " + highScore;
    }
    void ReloadScene()
    {
        Time.timeScale = 1f;
        MainMenuController.skipMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void ReturnToMainMenu()
    {
        MainMenuController.skipMenu = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
