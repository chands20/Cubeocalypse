using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI zombiesRemaining;
    [SerializeField] TextMeshProUGUI playerCoinText;
    [SerializeField] TextMeshProUGUI nextWaveText;

    //SHOP

    [SerializeField] GameObject shopView;
    [SerializeField] UnityEngine.UI.Button buyAmmoButton;
    [SerializeField] UnityEngine.UI.Button buyHealthButton;
    int waveNumber = 0;

    public int zombiesToSpawn = 10;
    public bool inBetweenWave = false;

    public Spawner spawner;
    public Player player;

    //Coins
    public int playerCoins = 0;


    private void Awake()
    {
        instance = this;
        nextWaveText.enabled = false;
        shopView.gameObject.SetActive(false);
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

}
