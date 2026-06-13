using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Inventory")]
    private InventoryManager inventoryManager;
    public List<ItemData> inventoryItems;
    private Transform itemParent;

    public float HP;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        itemParent = GameObject.Find("InventoryContent").transform;
        inventoryManager.CreateItem(0, inventoryItems);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerAnimator = GetComponent<Animator>();

       // EquipItem("RIFLE");
    }
    void Update()
    {
        Movement();
        Rotation();
        Jump();
        Shoot();
        Reload();

        playerAnimator.SetBool("isGrounded", groundChecker.isGrounded);
        if(Input.GetKeyDown(KeyCode.Tab) && !inventoryManager.inventoryPanel.activeSelf)
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventoryPanels();
        }
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
        if(Input.GetMouseButtonDown(0) && groundChecker.isGrounded == true)
        {
            playerAnimator.Play("Fire");
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown("r") && groundChecker.isGrounded == true)
        {
            playerAnimator.Play("Reload");
        }
    }
    private void OpenInventory()
    {
        //canMove = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        inventoryManager.inventoryPanel.SetActive(true);
        if(inventoryItems.Count > 0)
        {
            for(int i = 0; i<inventoryItems.Count; i++)
            {
                inventoryManager.InstantiatingItem(inventoryItems[i], itemParent, inventoryManager.inventorySlots);
            }
        }
    }

    private void CloseInventoryPanels()
    {
   // canMove = true;
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

   // foreach (GameObject slot in inventoryManager.currentChestSlots)
    //{
    //   Destroy(slot);
    //}
    foreach (GameObject slot in inventoryManager.inventorySlots)
    {
        Destroy(slot);
    }

    //inventoryManager.currentChestSlots.Clear();
    inventoryManager.inventorySlots.Clear();

    inventoryManager.inventoryPanel.SetActive(false);
    //inventoryManager.chestPanel.SetActive(false);
    }
    //private void EquipItem(string toolName)
    //{
    //    foreach (GameObject tool in equipableItems)
    //    {
    //        if (tool.name == toolName)
    //        {
    //            tool.SetActive(true);
    //            currentEquipedItem = tool;
    //            toolName = EQUIPE_NOT_SELECTED_TEXT;
    //        }
    //        else
    //        {
    //            tool.SetActive(false);
    //        }
    //    }
    //}
}
