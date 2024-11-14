using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f; // Movement speed of the player
    public float rotationSpeed = 10f; // Rotation speed of player
    private Camera mainCamera;   // Reference to the main camera
    private Animator animator;

    // Gun/Shooting 
    public Transform shootingPoint; // Position from which the raycast originates
    public float fireRate = 0.5f; // Time between shots
    public float shootingRange = 100f; // Range of the gun
    public int damage = 10; // Damage dealt to zombies
    private float nextFireTime = 0f;
    private int totalAmmo = 50;
    private int currentAmmo = 10;
    private bool isReloading = false;
    [SerializeField] AudioSource gunshotAudio; // gunshot

    public LineRenderer aimLineRenderer;
    public GameObject bulletTrailPrefab;

    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;  // Get the main camera reference
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Player movement
        MovePlayer();
        RotatePlayerToMouse();

        AimTracer();
        
        // Shooting
        if (Input.GetMouseButton(0) && Time.time > nextFireTime && !isReloading) // Left mouse button
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
        
        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
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

        // if movement walking animation
        if (moveDirection.magnitude > 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    void RotatePlayerToMouse()
    {
        // Get mouse position in the world
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Get the point on the ground where the mouse is pointing
            Vector3 targetPosition = hitInfo.point;
            targetPosition.y = transform.position.y; // Keep only the Y rotation

            // Rotate the player to face the target position
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void AimTracer()
    {
            RaycastHit hit;
            Vector3 endPosition;

            // Cast a ray from the shooting point forward
            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit, shootingRange))
            {
                endPosition = hit.point;
            }
            else
            {
                // If no hit, set the end position at max range
                endPosition = shootingPoint.position + shootingPoint.forward * shootingRange;
            }

            // Update LineRenderer positions
            aimLineRenderer.SetPosition(0, shootingPoint.position); // Start position
            aimLineRenderer.SetPosition(1, endPosition);            // End position
    }


    void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--; // Decrease current ammo by one
            GameManager.instance.UpdateAmmoText(currentAmmo, totalAmmo, isReloading);
            RaycastHit hit;

            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit, shootingRange))
            {
                if (hit.collider.CompareTag("Zombie"))
                {
                    Zombie zombieHealth = hit.collider.GetComponent<Zombie>();
                    if (zombieHealth != null)
                    {
                        zombieHealth.TakeDamage(damage); // Apply damage to the zombie
                    }
                }
            }
            gunshotAudio.Play();
        }
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break; // Prevent multiple reloads

        isReloading = true;
        Debug.Log("Reloading...");
        GameManager.instance.UpdateAmmoText(currentAmmo, totalAmmo, isReloading);

        yield return new WaitForSeconds(3f); // Wait for 3 seconds to simulate reload

        int maxMagazineSize = 10; // Assuming 10 is the max capacity
        int bulletsNeeded = maxMagazineSize - currentAmmo; // Calculate bullets needed to fill the magazine
        int bulletsToReload = Mathf.Min(bulletsNeeded, totalAmmo); // Take only what is needed or what is available

        currentAmmo += bulletsToReload; // Update current ammo
        totalAmmo -= bulletsToReload;   // Subtract from total ammo

        isReloading = false;
        Debug.Log("Reload complete");
        GameManager.instance.UpdateAmmoText(currentAmmo, totalAmmo, isReloading);
    }

}
