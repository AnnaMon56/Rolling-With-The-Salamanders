using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Difficulty")]
    [Range(0.25f, 4f)] public float difficulty = 1f;

    public float distanceTravelled = 0f;
    public bool isGameOver = false;

    private Transform player;
    private float startZ = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        startZ = player.position.z;
    }

    void Update()
    {
        if (!isGameOver && player != null)
            distanceTravelled = Mathf.Max(0f, Mathf.Round(player.position.z - startZ));
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! Distance: " + distanceTravelled);

        if (MenuManager.Instance != null)
            MenuManager.Instance.ShowGameOver();
        else
            Debug.Log("MenuManager.Instance is NULL at game over");
    }
}