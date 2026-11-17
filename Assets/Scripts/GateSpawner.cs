using UnityEngine;
using System.Collections.Generic;

public class GateSpawner : MonoBehaviour
{
    public GameObject gatePairPrefab;
    public Transform player;
    public float spawnInterval = 15f;
    public float spawnDistance = 80f;
    public float despawnBehind = 20f;
    public Material positiveMat;
    public Material negativeMat;

    [Header("Difficulty")]
    public int negativesAfterTotal = 5;   // no minus gates until band reaches this size

    private float timer = 0f;

    void Start()
    {
        timer = spawnInterval - 1f;   // first gate arrives ~4s in so you can build up fast
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval) { timer = 0f; SpawnGatePair(); }

        foreach (GameObject pair in GameObject.FindGameObjectsWithTag("GatePair"))
            if (pair != null && pair.transform.position.z < player.position.z - despawnBehind)
                Destroy(pair);
    }

    void SpawnGatePair()
    {
        Vector3 pos = new Vector3(0, 1f, player.position.z + spawnDistance);
        GameObject pair = Instantiate(gatePairPrefab, pos, Quaternion.identity);

        Gate[] gates = pair.GetComponentsInChildren<Gate>();
        if (gates.Length < 2) return;

        int total = BandManager.Instance != null ? BandManager.Instance.TotalMembers : 0;
        int negativeIndex = -1;
        if (total >= negativesAfterTotal && Random.value < 0.8f)  
            negativeIndex = Random.Range(0, 2);

        for (int i = 0; i < 2; i++)
        {
            var type = RandomInstrument();

            if (i == negativeIndex)
            {
                // NEGATIVE gate — only target instruments the player actually has
                var present = InstrumentsWithMembers();
                if (present.Count == 0)   // nothing to subtract → make it a positive add instead
                {
                    gates[i].SetEffect(type, "add", Random.Range(1, 4));
                    SetMat(gates[i], positiveMat);
                    continue;
                }
                type = present[Random.Range(0, present.Count)];
                string op = Random.value < 0.5f ? "subtract" : "divide";
                int value = op == "divide" ? Random.Range(2, 4) : Random.Range(1, 4);
                gates[i].SetEffect(type, op, value);
                SetMat(gates[i], negativeMat);
            }
            else
            {
                // POSITIVE gate
                int current = BandManager.Instance != null ? BandManager.Instance.GetCount(type) : 0;
                string op = (current >= 1 && Random.value < 0.4f) ? "multiply" : "add";
                int value = op == "multiply" ? Random.Range(2, 3) : Random.Range(1, 5);
                gates[i].SetEffect(type, op, value);
                SetMat(gates[i], positiveMat);
            }
        }
    }

    BandManager.InstrumentType RandomInstrument()
    {
        var vals = (BandManager.InstrumentType[])System.Enum.GetValues(typeof(BandManager.InstrumentType));
        return vals[Random.Range(0, vals.Length)];
    }

    void SetMat(Gate g, Material m)
    {
        Renderer r = g.GetComponent<Renderer>();
        if (r != null && m != null) r.material = m;
    }

    List<BandManager.InstrumentType> InstrumentsWithMembers()
    {
        var list = new List<BandManager.InstrumentType>();
        foreach (BandManager.InstrumentType t in System.Enum.GetValues(typeof(BandManager.InstrumentType)))
            if (BandManager.Instance != null && BandManager.Instance.GetCount(t) > 0) list.Add(t);
        return list;
    }
}