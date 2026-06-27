using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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


    [Header("Equipment")]
    public const string EQUIPE_NOT_SELECTED_TEXT = "EquipeNotSelected";
    [HideInInspector]
    public string itemYouCanEquipeName = EQUIPE_NOT_SELECTED_TEXT;
    [SerializeField] GameObject[] equipableItems;
    [SerializeField] private GameObject currentEquipedItem;

    private RifleShooter rifleShooterScript;

    //public string itemName = "BULLETS";

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        itemParent = GameObject.Find("InventoryContent").transform;
        inventoryManager.CreateItem(0, inventoryItems);
        inventoryManager.CreateItem(1, inventoryItems);

        rifleShooterScript = GameObject.Find("Gun").GetComponent<RifleShooter>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerAnimator = GetComponent<Animator>();

        // EquipItem("RIFLE");
        GetItemCount("BULLETS");
    }
    void Update()
    {
        Movement();
        Rotation();
        Jump();
        //Shoot();
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

    public void Shoot()
    {
        //if(Input.GetMouseButtonDown(0) && groundChecker.isGrounded == true)
        //{
            playerAnimator.Play("Fire");

        //}
    }

    private void Reload()
    {
        // нужна ссылка из rifleshooter
        if (rifleShooterScript.isReloading)
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
    private void EquipItem(string toolName)
    {
        foreach (GameObject tool in equipableItems)
        {
            if (tool.name == toolName)
            {
                tool.SetActive(true);
                currentEquipedItem = tool;
                toolName = EQUIPE_NOT_SELECTED_TEXT;
            }
            else
            {
                tool.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BulletPack"))
        {
            inventoryManager.CreateItem(1, inventoryItems);
            rifleShooterScript.AddAmmo();
            Destroy(other.gameObject);
        }
    }

    public void ModifyItemCount(string itemName)
    {
        foreach (ItemData item in inventoryItems)    // пошук в масиві конкретного блока
        {
            if (item.name == itemName)               // перевірка по імені
            {
                item.count--;
                if (item.count <= 0)                  // перевірка на закінчення в інвентарі
                {
                    inventoryItems.Remove(item);    // видалення предмету зі списку
                    EquipItem(inventoryItems[0].name);      // екіпірування кірки
                }
                break;
            }
        }
    }

    //public int GetCurrentAmmo()
    //{

    //}

    public int GetItemCount(string itemName)
    {
        foreach (ItemData item in inventoryItems)
        {
            if (item.name == itemName)
            {
                //Debug.Log(item.count);
                return item.count;
            }
        }
        return 0;
    }
}
