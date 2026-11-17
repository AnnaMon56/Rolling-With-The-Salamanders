using UnityEngine;
using TMPro;

public class Gate : MonoBehaviour
{
    public BandManager.InstrumentType instrument;
    public string effectType;   // "add", "subtract", "multiply", "divide"
    public int effect;
    public TextMeshPro label;
    private bool triggered = false;
    public GameObject[] instrumentModels;   // 4 models, enum order, all children of the gate
    

    public void SetEffect(BandManager.InstrumentType type, string op, int value)
    {
        Debug.Log($"{name} → {type}");
        instrument = type; effectType = op; effect = value;
        UpdateLabel();
        foreach (var m in instrumentModels) if (m != null) m.SetActive(false);
        // in SetEffect, after UpdateLabel():
        for (int m = 0; m < instrumentModels.Length; m++)
            if (instrumentModels[m] != null)
                instrumentModels[m].SetActive(m == (int)instrument);

    }

    void UpdateLabel()
    {
        if (label == null) return;
        string sym = Symbol(instrument);
        switch (effectType)
        {
            case "add":      label.text = "+" + effect; break;
            case "subtract": label.text = "-" + effect; break;
            case "multiply": label.text = "×" + effect; break;
            case "divide":   label.text = "÷" + effect; break;
        }
    }

    string Symbol(BandManager.InstrumentType t)
    {
        switch (t)
        {
            case BandManager.InstrumentType.Percussion:   return "Drums";
            case BandManager.InstrumentType.Keyboard:     return "Keys";
            case BandManager.InstrumentType.Bass:         return "Bass";
            case BandManager.InstrumentType.RhythmGuitar: return "Rhythm";
        }
        return "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        Gate[] siblings = transform.parent.GetComponentsInChildren<Gate>();
        foreach (Gate g in siblings) g.triggered = true;   // block the other gate

        ApplyEffect();
        if (effectType == "subtract" || effectType == "divide")
            other.GetComponent<PlayerFlash>()?.Flash();
        Destroy(gameObject);
    }

    void ApplyEffect()
    {
        var bm = BandManager.Instance;
        int current = bm.GetCount(instrument);
        int result = current;
        switch (effectType)
        {
            case "add":      result = current + effect; break;
            case "subtract": result = current - effect; break;
            case "multiply": result = current * effect; break;
            case "divide":   result = Mathf.Max(0, current / effect); break;
        }
        int delta = result - current;
        if (delta >= 0) bm.AddMembers(instrument, delta);
        else bm.RemoveMembers(instrument, -delta);
    }
}