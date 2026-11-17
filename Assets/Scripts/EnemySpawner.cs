using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject obstaclePrefab;
    public Transform player;

    public float spawnInterval = 8f;
    public float spawnDistance = 60f;
    public float corridorWidth = 3.5f;

    // chance of an enemy; obstacle fills the rest
    [Range(0, 1)] public float enemyChance = 0.5f;

    [Header("Difficulty ramp")]
    public float minSpawnInterval = 2.5f;      // fastest spawns get
    public float intervalDropPer100m = 0.5f;   // how much quicker per 100m travelled

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= CurrentInterval())
        {
            timer = 0f;
            Spawn();
        }
    }

    // spawns speed up the further you get
    float CurrentInterval()
    {
        int dist = GameManager.Instance != null
            ? Mathf.FloorToInt(GameManager.Instance.distanceTravelled)
            : 0;
        return Mathf.Max(minSpawnInterval, spawnInterval - (dist / 100) * intervalDropPer100m);
    }

    void Spawn()
    {
        if (BandManager.Instance == null || BandManager.Instance.TotalMembers <= 0) return;

        float roll = Random.value;
        float z = player.position.z + spawnDistance;

        if (roll < enemyChance && enemyPrefab != null)
        {
            // centre of the corridor, facing the oncoming player
            Vector3 pos = new Vector3(0f, 0.5f, z);
            Instantiate(enemyPrefab, pos, Quaternion.Euler(0, 180, 0));
        }
        else if (obstaclePrefab != null)
        {
            Vector3 pos = new Vector3(0f, 0.5f, z);
            Instantiate(obstaclePrefab, pos, obstaclePrefab.transform.rotation);
        }
    }
}