using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    [Header("Tile Setup")]
    public GameObject tilePrefab;
    public int poolSize = 10;
    public float tileLength = 20f;
    public int tilesAhead = 8;

    [Header("Materials")]
    public Material tileMatA;
    public Material tileMatB;

    [Header("References")]
    public Transform player;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeTiles = new List<GameObject>();
    private float spawnZ = -40f;
    private float safeZone = 30f;
    private int tileCount = 0;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject t = Instantiate(tilePrefab);
            t.SetActive(false);
            pool.Enqueue(t);
        }

        for (int i = 0; i < tilesAhead; i++)
            SpawnTile();
    }

    void Update()
    {
        if (player.position.z + (tilesAhead * tileLength) > spawnZ)
            SpawnTile();

        RecycleTiles();
    }

    void SpawnTile()
    {
        GameObject tile = GetFromPool();
        tile.transform.position = new Vector3(0, 0, spawnZ);
        tile.SetActive(true);

        // alternate materials
        Material mat = (tileCount % 2 == 0) ? tileMatA : tileMatB;
        tile.GetComponent<Renderer>().material = mat;
        tileCount++;

        activeTiles.Add(tile);
        spawnZ += tileLength;
    }

    void RecycleTiles()
    {
        for (int i = activeTiles.Count - 1; i >= 0; i--)
        {
            if (activeTiles[i].transform.position.z < player.position.z - safeZone)
            {
                ReturnToPool(activeTiles[i]);
                activeTiles.RemoveAt(i);
            }
        }
    }

    GameObject GetFromPool()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        GameObject t = Instantiate(tilePrefab);
        t.SetActive(false);
        return t;
    }

    void ReturnToPool(GameObject tile)
    {
        tile.SetActive(false);
        pool.Enqueue(tile);
    }
}