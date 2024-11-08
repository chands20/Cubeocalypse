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
    int waveNumber = 0;

    public int zombiesToSpawn = 10;

    public Spawner spawner;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartNextWave(); // Start the first wave at the beginning
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
        // Wait a few seconds before starting the next wave (optional delay can be added here)
        Invoke(nameof(StartNextWave), 5f);
    }

    public void UpdateZombiesRemaining(int zombiesLeft)
    {
        zombiesRemaining.text = "Zombies Remaining: " + zombiesLeft;
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
