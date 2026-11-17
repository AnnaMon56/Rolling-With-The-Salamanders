using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    public GameObject[] bulletPrefabs;

    [Header("Base Stats")]
    public float baseFireRate = 0.6f;
    public float minFireRate = 0.15f;    // raised from 0.1 — main ceiling nerf
    public int baseDamage = 1;
    public float bulletSpawnForwardOffset = 3f;

    [Header("Per-Type Tuning (all still peak at 20 members)")]
    public float fireRatePerPercussion = 0.022f;  // ramps to ~min at 20
    public int damagePerBass = 2;                  // peak damage 41 — lower to 1 for tankier bosses
    public int bulletsPerRhythm = 6;               // +1 bullet per 6 → 4 bullets at 20 (was 5)
    public float keyboardRangePerUnit = 0.4f;      // range only, doesn't affect DPS

    [Header("Full Band Reward")]
    public float fullBandBulletSpeedBonus = 25f;

    private PlayerController pc;
    private float fireTimer = 0f;

    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    void Update()
    {
        int perc = Count(BandManager.InstrumentType.Percussion);
        float fireRate = Mathf.Max(minFireRate, baseFireRate - perc * fireRatePerPercussion);

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefabs == null || bulletPrefabs.Length == 0) return;

        int bass     = Count(BandManager.InstrumentType.Bass);
        int rhythm   = Count(BandManager.InstrumentType.RhythmGuitar);
        int keyboard = Count(BandManager.InstrumentType.Keyboard);

        int damage      = baseDamage + bass * damagePerBass;
        int bulletCount = 1 + rhythm / bulletsPerRhythm;
        float lifeBonus = keyboard * keyboardRangePerUnit;

        bool full = BandManager.Instance != null && BandManager.Instance.IsFull;

        for (int n = 0; n < bulletCount; n++)
        {
            float xSpread = (bulletCount == 1) ? 0f : (n - (bulletCount - 1) / 2f) * 0.5f;

            Vector3 spawnPos = new Vector3(
                transform.position.x + xSpread,
                transform.position.y + 0.5f,
                transform.position.z + bulletSpawnForwardOffset
            );

            GameObject prefab = bulletPrefabs[Random.Range(0, bulletPrefabs.Length)];
            GameObject b = Instantiate(prefab, spawnPos, Quaternion.identity);

            Bullet bullet = b.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.damage = damage;
                bullet.speed = (pc != null ? pc.forwardSpeed : 8f) + 15f;
                if (full) bullet.speed += fullBandBulletSpeedBonus;
                bullet.lifetime += lifeBonus;
            }
        }
    }

    int Count(BandManager.InstrumentType t)
    {
        return BandManager.Instance != null ? BandManager.Instance.GetCount(t) : 0;
    }
}