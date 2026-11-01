using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int lives = 3;
    public int score;
    [HideInInspector] public int maxLives;
    public int Best { get; private set; }
    public bool IsGameOver { get; private set; }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        maxLives = lives;
        Best = PlayerPrefs.GetInt("best", 0);
    }

    void Start()
    {
        AudioManager.PlayMusic();
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        score += amount;
        if (score > Best) Best = score;
    }

    public void LoseLife()
    {
        if (IsGameOver) return;
        lives--;
        if (lives <= 0)
        {
            lives = 0;
            IsGameOver = true;
            Time.timeScale = 0f;
            PlayerPrefs.SetInt("best", Best);
            PlayerPrefs.Save();
            AudioManager.StopMusic();
            AudioManager.Play("sfx_gameover", 1f, 0f);
        }
    }

    void Update()
    {
        if (!IsGameOver) return;
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if ((kb != null && kb.rKey.wasPressedThisFrame) ||
            (mouse != null && mouse.leftButton.wasPressedThisFrame))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
