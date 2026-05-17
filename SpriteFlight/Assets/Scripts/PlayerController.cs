using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;    
    public float thrustForce = 1f;
    
    public GameObject boosterFlame;
    Rigidbody2D rb;
    public UIDocument uiDocument;
    private Label scoreText;
    public GameObject explosionEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        Instantiate(explosionEffect, transform.position, transform.rotation);
    }
}
