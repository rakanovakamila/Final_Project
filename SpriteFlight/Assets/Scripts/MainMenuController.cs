using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private UIDocument mainMenuUIDoc;
    private Button startButton;
    private Button quitButton;
    public PlayerController playerController;
    public static bool skipMenu = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuUIDoc = GetComponent<UIDocument>();
        startButton = mainMenuUIDoc.rootVisualElement.Q<Button>("StartButton");
        quitButton = mainMenuUIDoc.rootVisualElement.Q<Button>("QuitButton");

        startButton.clicked += StartGame;
        quitButton.clicked += QuitGame;

        if (skipMenu)
        {
            Time.timeScale = 1f;
   
            if (playerController != null)
            {
                playerController.StartGameLoop();
            }
            gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    void StartGame()
    {
        Time.timeScale = 1f;
        skipMenu = true;

        if(playerController != null)
        {
            playerController.StartGameLoop();
        }
        gameObject.SetActive(false);
    }
    
    void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("Game Exited, rest in piece...");
    }
}
