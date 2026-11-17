using System.Collections.Generic;
using UnityEngine;

public class BandManager : MonoBehaviour
{
    public static BandManager Instance;

    public enum InstrumentType { Percussion, Keyboard, Bass, RhythmGuitar }

    [Header("Band Settings")]
    public int startingMembers = 3;   // small starting buffer so you're not instantly dead
    public int maxPerType = 20;

    [Header("Difficulty")]
    // fraction of your CURRENT band lost per point of ENEMY damage
    [Range(0f, 0.1f)] public float lossPerHitPoint = 0.03f;

    // minimum troops removed per hit — the early-game teeth.
    // higher = losing a few members actually hurts when your band is tiny.
    public int minLossPerHit = 2;

    private Dictionary<InstrumentType, int> counts = new Dictionary<InstrumentType, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        foreach (InstrumentType t in System.Enum.GetValues(typeof(InstrumentType)))
            counts[t] = 0;

        counts[InstrumentType.Percussion] = startingMembers;
    }

    // ---- reading the band ----
    public int GetCount(InstrumentType type) => counts[type];
    public bool Has(InstrumentType type) => counts[type] > 0;

    public int TotalMembers
    {
        get { int sum = 0; foreach (var v in counts.Values) sum += v; return sum; }
    }

    public int MaxTotal => maxPerType * System.Enum.GetValues(typeof(InstrumentType)).Length;
    public bool IsFull => TotalMembers >= MaxTotal;

    // ---- changing the band ----
    public void AddMembers(InstrumentType type, int amount)
    {
        counts[type] = Mathf.Clamp(counts[type] + amount, 0, maxPerType);
        Debug.Log($"{type}: {counts[type]}  (total troops {TotalMembers})");
    }

    public void RemoveMembers(InstrumentType type, int amount)
    {
        counts[type] = Mathf.Max(0, counts[type] - amount);
        CheckGameOver();
    }

    // enemies: loss scales with band size AND their damage, floored so early hits still sting
    public void LoseMembers(int amount)
    {
        int scaled = Mathf.Max(minLossPerHit, Mathf.RoundToInt(TotalMembers * lossPerHitPoint * amount));
        RemoveRandomTroops(scaled);
        CheckGameOver();
    }

    // obstacles: straight percentage of the band, also floored
    public void LosePercent(float fraction)
    {
        int loss = Mathf.Max(minLossPerHit, Mathf.RoundToInt(TotalMembers * fraction));
        RemoveRandomTroops(loss);
        CheckGameOver();
    }

    private void RemoveRandomTroops(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (TotalMembers <= 0) break;

            List<InstrumentType> pool = new List<InstrumentType>();
            foreach (var kv in counts)
                for (int c = 0; c < kv.Value; c++) pool.Add(kv.Key);

            if (pool.Count == 0) break;
            InstrumentType victim = pool[Random.Range(0, pool.Count)];
            counts[victim]--;
        }
    }

    void CheckGameOver()
    {
        if (TotalMembers <= 0)
        {
            Debug.Log("GAME OVER — band is gone!");
            if (GameManager.Instance != null) GameManager.Instance.TriggerGameOver();
            else Time.timeScale = 0f;
        }
    }
}