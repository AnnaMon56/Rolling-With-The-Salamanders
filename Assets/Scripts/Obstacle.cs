using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Range(0f, 0.5f)] public float percentLost = 0.15f;  // % of the band removed on hit

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        BandManager.Instance?.LosePercent(percentLost);
        other.GetComponent<PlayerFlash>()?.Flash();
        Destroy(gameObject);
    }
}