using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -5);
    public float sensitivity = 5f;
    public float followSpeed = 10f;
    public float minY = -20f;
    public float maxY = 20f;

    private float yaw;    // Horizontal rotation
    private float pitch;  // Vertical rotation

    void Start()
    {
        if (player == null)
            player = GameObject.Find("Player")?.transform;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY; // Inverted Y
        pitch = Mathf.Clamp(pitch, minY, maxY);

        // Apply rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Smooth follow
        Vector3 desiredPosition = player.position + rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.LookAt(player.position + Vector3.up * 1.5f); // Adjust target height if needed
    }
}
