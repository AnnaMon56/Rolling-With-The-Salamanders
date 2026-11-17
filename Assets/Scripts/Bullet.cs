using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 40f;
    public int damage = 1;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // move in world forward (positive Z) regardless of rotation
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Bullet hit: {other.name} tag={other.tag}");
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;   // stop physics moving/bouncing it
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;    // pass through + fire OnTriggerEnter
    }
}
