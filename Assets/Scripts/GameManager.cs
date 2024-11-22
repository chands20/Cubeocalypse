using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    // UI
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI zombiesRemaining;
    [SerializeField] TextMeshProUGUI playerCoinText;
    [SerializeField] TextMeshProUGUI nextWaveText;
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI gameOverText;

    //SHOP

    [SerializeField] GameObject shopView;
    [SerializeField] UnityEngine.UI.Button buyAmmoButton;
    [SerializeField] UnityEngine.UI.Button buyHealthButton;
    [SerializeField] TextMeshProUGUI buyHealthButtonText;
    public int waveNumber = 0;


    // Spawner
    public int zombiesToSpawn = 10;
    public bool inBetweenWave = false;

    public Spawner spawner;

    //Player
    public Player player;
    public bool gameOver = false;

    //Coins
    public int playerCoins = 0;


    // Main Menu
    [SerializeField] GameObject mainMenu;
    public bool isMainMenuActive;

    // High Score
    [SerializeField] TextMeshProUGUI highScoreText;
    private int highScore = 0;



    private void Awake()
    {
        instance = this;
        nextWaveText.enabled = false;
        shopView.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore + " Waves";
        }
        isMainMenuActive = true;
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartNextWave(); // Start the first wave at the beginning
    }

    // Update is called once per frame
    void Update()
    {
        if (inBetweenWave)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartNextWave();
                nextWaveText.enabled = false;
                //make shop disappear
            }
        }

        // pause game while main menu is active
        if (isMainMenuActive)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        // Start Game Over
        if (Input.GetButtonDown("Submit") && gameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    public void StartNextWave()
    {
        inBetweenWave = false;
        waveNumber++;
        zombiesToSpawn += 2;
        waveText.text = "Wave: " + waveNumber;
        spawner.StartWave(waveNumber);
        shopView.gameObject.SetActive(false);
    }

    public void OnWaveComplete()
    {
        inBetweenWave = true;
        nextWaveText.enabled = true;
        shopView.gameObject.SetActive(true);
    }

    public void UpdateZombiesRemaining(int zombiesLeft)
    {
        zombiesRemaining.text = "Zombies Remaining: " + zombiesLeft;
    }


    public void UpdateAmmoText(int current, int total, bool reloading)
    {
        if (reloading)
        {
            ammoText.text = "Reloading...";
        }
        else
        {
            ammoText.text = current + "/" + total;
        }
        
    }

    void UpdateCoinText()
    {
        playerCoinText.text = "Coins: " + playerCoins;
    }

    public void PickedUpCoin(int amount)
    {
        playerCoins += amount;
        UpdateCoinText();
    }

    public void BuyAmmo()
    {
        if (playerCoins >= 5)
        {
            player.AddAmmo();
            playerCoins -= 5;
        }
        UpdateCoinText();
    }

    public void BuyHealth()
    {
        if(playerCoins >= 10 && player.playerHealth != 3)
        {
            player.AddHealth();
            playerCoins -= 10;
        }
        UpdateCoinText();
    }

    public void GameOver()
    {
        if (waveNumber > highScore)
        {
            highScore = waveNumber;
            PlayerPrefs.SetInt("HighScore", highScore); // Save the high score into memory
        }
        player.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        gameOver = true;

    }

    public void UpdatePlayerHealthText(int health)
    {
        if (health == 3)
        {
            playerHealthText.text = "Health: <3 <3 <3";
        }
        else if (health == 2)
        {
            playerHealthText.text = "Health: <3 <3";
        }
        else if(health == 1)
        {
            playerHealthText.text = "Health: <3";
        }
        else
        {
            playerHealthText.text = "Health: ";
        }
    }

    public void StartGame()
    {
        isMainMenuActive = false;
        mainMenu.gameObject.SetActive(false);
    }

}
