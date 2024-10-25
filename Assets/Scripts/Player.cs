using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f; // Movement speed of the player
    public float rotationSpeed = 10f; // Rotation speed of player
    private Camera mainCamera;   // Reference to the main camera
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;  // Get the main camera reference
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Get the WASD input
        float hInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        float vInput = Input.GetAxis("Vertical");   // W/S or Up/Down arrow keys

        // Get the camera's forward (Z axis) and right (X axis) vectors
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Project forward and right vectors onto the XZ plane (ignore Y axis)
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction relative to the camera
        Vector3 moveDirection = (cameraForward * vInput + cameraRight * hInput).normalized;

        // Move the player
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotate the player if there is movement input
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y , 0f);  // Only rotate around Y-axis

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
}
