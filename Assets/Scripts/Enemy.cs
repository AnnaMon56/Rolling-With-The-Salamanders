using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Health scaling")]
    public int baseHealth = 18;                // meaningful even at zero band
    public int healthPerTroop = 5;             // tougher as YOUR band grows — kills the mid-game plateau
    public float healthGrowthPer1000m = 1.4f;  // HP multiplies by this every 1000m (exponential → runs end)

    public int troopDamage = 3;
    public TextMeshPro healthLabel;

    private int maxHealth;
    private int currentHealth;

    void Start()
    {
        int dist = GameManager.Instance != null
            ? Mathf.FloorToInt(GameManager.Instance.distanceTravelled) : 0;
        int band = BandManager.Instance != null
            ? BandManager.Instance.TotalMembers : 0;
        float diff = GameManager.Instance != null
            ? GameManager.Instance.difficulty : 1f;

        float mult = Mathf.Pow(healthGrowthPer1000m, dist / 1000f);
        maxHealth = Mathf.Max(1, Mathf.RoundToInt((baseHealth + band * healthPerTroop) * mult * diff));

        currentHealth = maxHealth;
        UpdateLabel();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateLabel();
        if (currentHealth <= 0) Die();
    }

    void UpdateLabel()
    {
        if (healthLabel != null) healthLabel.text = currentHealth.ToString();
    }

    void Die() { Destroy(gameObject); }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BandManager.Instance?.LoseMembers(troopDamage);
            other.GetComponent<PlayerFlash>()?.Flash();
            Die();
        }
    }
}