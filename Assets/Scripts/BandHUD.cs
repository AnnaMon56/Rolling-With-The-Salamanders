using UnityEngine;
using TMPro;

public class BandHUD : MonoBehaviour
{
    [System.Serializable]
    public class Row
    {
        public BandManager.InstrumentType instrument;
        public TMP_Text label;
    }

    [Header("One row per instrument (wire in Inspector)")]
    public Row[] rows;

    [Header("Optional")]
    public TMP_Text totalLabel;
    public TMP_Text distanceLabel;

    void Update()
    {
        if (BandManager.Instance == null) return;

        foreach (var r in rows)
            if (r.label != null)
                r.label.text = $"{r.instrument}: {BandManager.Instance.GetCount(r.instrument)}";

        if (totalLabel != null)
            totalLabel.text = "Band: " + BandManager.Instance.TotalMembers;

        if (distanceLabel != null && GameManager.Instance != null)
            distanceLabel.text = Mathf.RoundToInt(GameManager.Instance.distanceTravelled) + "m";
    }
}