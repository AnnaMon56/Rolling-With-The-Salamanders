using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 6, -8);

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            target.position + offset,
            Time.deltaTime * 10f
        );
    }
}