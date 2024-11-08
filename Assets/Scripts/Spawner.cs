using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] int spawnWidth;
    [SerializeField] int spawnDepth;
    public GameObject zombiePrefab;         // Zombie prefab to spawn
    public Vector2 spawnAreaSize;
    public float spawnInterval = 5f;        // Time in seconds between each spawn


    private int zombiesSpawned = 0;
    private int zombiesLeftToKill = 0;

    void Start()
    {
     spawnAreaSize = new Vector2(spawnWidth, spawnDepth); // Width and Depth of spawn area
    // Start spawning zombies at intervals
    InvokeRepeating(nameof(SpawnZombie), 0f, spawnInterval);
    }

    void SpawnZombie()
    {
        // Generate a random position within the spawn area
        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            transform.position.y,
            transform.position.z + Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );

        // Instantiate a new zombie at the calculated position
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.Euler(-90, 0, 0));

        Zombie zombieScript = zombie.GetComponent<Zombie>();
        if (zombieScript != null)
        {
            zombieScript.OnZombieDeath += OnZombieDeath;
        }
    }

    public void StartWave(int waveNumber)
    {
        GameManager.instance.zombiesToSpawn = 5 + (waveNumber - 1) * 2; // Calculate number of zombies to spawn
        zombiesLeftToKill = GameManager.instance.zombiesToSpawn; // Track zombies for completion
        zombiesSpawned = 0;

        GameManager.instance.UpdateZombiesRemaining(zombiesLeftToKill);
        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        while (zombiesSpawned < GameManager.instance.zombiesToSpawn)
        {
            SpawnZombie();
            zombiesSpawned++;

            if (zombiesSpawned >= GameManager.instance.zombiesToSpawn)
            {
                yield break; // Stop the coroutine when the limit is reached
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void OnZombieDeath()
    {
        zombiesLeftToKill--;

        GameManager.instance.UpdateZombiesRemaining(zombiesLeftToKill);

        if (zombiesLeftToKill <= 0)
        {
            GameManager.instance.OnWaveComplete(); // Notify GameManager when the wave is complete
        }
    }
}
