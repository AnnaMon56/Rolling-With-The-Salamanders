using UnityEngine;

public class SpeedScaler : MonoBehaviour
{
    [Header("Scaling")]
    public PlayerController player;
    public float speedIncreasePerSecond = 0.05f;
    public float maxSpeed = 25f;

    void Update()
    {
        if (player.forwardSpeed < maxSpeed)
            player.forwardSpeed += speedIncreasePerSecond * Time.deltaTime;
    }
}
