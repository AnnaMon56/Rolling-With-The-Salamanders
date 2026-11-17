using System.Collections.Generic;
using UnityEngine;

public class BandFormation : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [System.Serializable]
    public class MemberType
    {
        public BandManager.InstrumentType instrument;
        public GameObject prefab;
        public float laneX;
        [HideInInspector] public List<GameObject> pool = new List<GameObject>();
    }

    [Header("One entry per instrument (set in Inspector)")]
    public MemberType[] types;

    [Header("Formation")]
    public int maxPerType = 20;
    public float rowSpacing = 0.7f;        // was 1.2 — smaller = members sit closer behind you
    public int perRow = 3;
    public float followSpeed = 12f;        // was 8 — higher = they keep up more tightly
    public float startBehind = 1.2f;       // gap between you and the first row

    void Start()
    {
        foreach (var t in types)
            for (int i = 0; i < maxPerType; i++)
            {
                GameObject m = Instantiate(t.prefab, transform);
                Rigidbody rb = m.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
                Collider col = m.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                m.SetActive(false);
                t.pool.Add(m);
            }
    }

    void Update()
    {
        foreach (var t in types)
        {
            int count = Mathf.Min(BandManager.Instance.GetCount(t.instrument), maxPerType);
            for (int i = 0; i < t.pool.Count; i++)
            {
                bool active = i < count;
                if (t.pool[i].activeSelf != active) t.pool[i].SetActive(active);
                if (!active) continue;

                Vector3 target = player.position + GetOffset(i, t.laneX);
                target.x = Mathf.Clamp(target.x, -3.5f, 3.5f);
                t.pool[i].transform.position = Vector3.Lerp(
                    t.pool[i].transform.position, target, Time.deltaTime * followSpeed);
            }
        }
    }

    Vector3 GetOffset(int index, float laneX)
    {
        float corridorLimit = 4f;
        int row = index / perRow;
        int col = index % perRow;
        float x = laneX + (col - (perRow - 1) / 2f) * 0.6f;
        x = Mathf.Clamp(x, -corridorLimit, corridorLimit);
        float z = -startBehind - row * rowSpacing;
        return new Vector3(x, 0f, z);
    }
}