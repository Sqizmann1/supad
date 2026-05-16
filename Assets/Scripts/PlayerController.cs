using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraPivot;
    public float mouseSensitivity = 200f;
    public float verticalLimit = 80f;

    float xRotation = 0f;

    public float speed = 5f;
    public float jumpForce = 3f;

    private Rigidbody rb;
    private Vector3 movement;

    public GroundChecker groundChecker;

    private Animator playerAnimator;

    public float HP;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();

        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        Movement();
        Rotation();
        Jump();
        Shoot();
        Reload();

        playerAnimator.SetBool("isGrounded", groundChecker.isGrounded);
    }

    void FixedUpdate()
    {
        Vector3 velocity = movement * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void Movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        movement = (transform.forward * v + transform.right * h).normalized;

        if (!groundChecker.isGrounded)
        {
            h = 0;
            v = 0;
        }

        playerAnimator.SetFloat("MoveX", h);
        playerAnimator.SetFloat("MoveY", v);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundChecker.isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLimit, verticalLimit);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void Shoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
            playerAnimator.Play("Fire");
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown("r"))
        {
            playerAnimator.Play("Reload");
        }
    }
}
