using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI zombiesRemaining;
    [SerializeField] TextMeshProUGUI playerCoinText;
    int waveNumber = 0;

    public int zombiesToSpawn = 10;
    public bool inBetweenWave = true;

    public Spawner spawner;

    //Coins
    public int playerCoins = 0;


    private void Awake()
    {
        instance = this;
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
                //make button invisible
                //make shop disappear
            }
        }
    }

    public void StartNextWave()
    {
        waveNumber++;
        zombiesToSpawn += 2;
        waveText.text = "Wave: " + waveNumber;
        spawner.StartWave(waveNumber);
    }

    public void OnWaveComplete()
    {
        inBetweenWave = true;
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

}
