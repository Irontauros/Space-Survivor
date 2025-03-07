using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 4f;

    private float currentX = 0f;
    public Vector3 cameraOffset;
    public bool isGameOver = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraOffset = new Vector3(-0.29f, 2.0f, -0.8f);
    }

    void Update()
    {
        if (isGameOver || PauseManager.isPaused) return; // Stop camera movement if game over or paused

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        player.rotation = Quaternion.Euler(0, currentX, 0);
        transform.position = player.position + player.rotation * cameraOffset;
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }


    public void SetGameOver(bool state)
    {
        isGameOver = state;

        if (state)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;                
        }
    }
}
