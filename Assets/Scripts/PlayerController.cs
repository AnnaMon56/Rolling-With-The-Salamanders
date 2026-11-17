using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 8f;
    public float lateralSpeed = 8f;
    public float jumpForce = 5f;
    public float corridorWidth = 4f;

    [Header("Touch")]
    public float dragSensitivity = 0.02f;
    public float tapMaxMove = 30f;

    private Rigidbody rb;
    private bool isGrounded;

    private int activeFinger = -1;
    private Vector2 lastTouchPos;
    private Vector2 touchStartPos;
    private bool jumpQueued;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);

        float input = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input = 1f;
        float move = input * lateralSpeed * Time.deltaTime;

        bool keyboardJump = Input.GetKeyDown(KeyCode.Space);
        HandleTouch(ref move);

        transform.Translate(move, 0, 0);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -corridorWidth, corridorWidth);
        transform.position = pos;

        if ((keyboardJump || jumpQueued) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        jumpQueued = false;
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }

    void LateUpdate()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * 2f * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.linearVelocity += Vector3.up * Physics.gravity.y * 1f * Time.deltaTime;
    }

    void HandleTouch(ref float move)
    {
        if (Input.touchCount == 0) { activeFinger = -1; return; }
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                activeFinger = t.fingerId;
                lastTouchPos = t.position;
                touchStartPos = t.position;
            }
            else if (t.fingerId == activeFinger)
            {
                if (t.phase == TouchPhase.Moved)
                {
                    float dx = t.position.x - lastTouchPos.x;
                    move += dx * dragSensitivity;
                    lastTouchPos = t.position;
                }
                else if (t.phase == TouchPhase.Ended)
                {
                    if (Vector2.Distance(t.position, touchStartPos) < tapMaxMove) jumpQueued = true;
                    activeFinger = -1;
                }
            }
        }
    }
}