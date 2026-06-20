using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class RifleShooter : MonoBehaviour
{
    private PlayerController controllerScript;
    [Header("References")]
    public Transform muzzle;           // точка выхода пули (дуло)
    public Camera playerCamera;        // камера игрока

    [Header("Bullet Settings")]
    public float bulletSpeed = 80f;    // м/с
    public float bulletSpread = 0.5f;  // разброс (градусы)

    [Header("Fire Settings")]
    public float fireRate = 0.1f;      // секунд между выстрелами
    public int bulletsPerShot = 1;     // 1 = винтовка, 8+ = дробовик

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 2f;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    private int totalAmmo;

    void Start()
    {
        controllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        totalAmmo = controllerScript.GetItemCount("BULLETS");
        Debug.Log(totalAmmo);
        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (isReloading) return;
        if (totalAmmo > 0 )
        {
            if (currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
                return;
            }
        }
        

        // Автоматическая стрельба — зажатая кнопка
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && totalAmmo > 0)
            Shoot();
    }

    void Shoot()
    {
        if (BulletPool.Instance == null) return;

        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        totalAmmo--;
        controllerScript.ModifyItemCount("BULLETS");

        for (int i = 0; i < bulletsPerShot; i++)
        {
            // Направление: из камеры в центр экрана + разброс
            Vector3 direction = GetShootDirection();

            var bullet = BulletPool.Instance.Get(muzzle.position, Quaternion.LookRotation(direction));
            bullet.Launch(direction * bulletSpeed);
        }

        // TODO: Muzzle flash, звук, анимация
    }

    Vector3 GetShootDirection()
    {
        // Ray из центра камеры — точнее для TPS
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            targetPoint = hit.point;
        else
            targetPoint = ray.origin + ray.direction * 500f;

        Vector3 dir = (targetPoint - muzzle.position).normalized;

        // Разброс
        if (bulletSpread > 0f)
        {
            dir += new Vector3(
                Random.Range(-bulletSpread, bulletSpread) * 0.01f,
                Random.Range(-bulletSpread, bulletSpread) * 0.01f,
                0f
            );
            dir.Normalize();
        }

        return dir;
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
    }

    // Для UI
    public string GetAmmoText() => $"{currentAmmo} / {magazineSize}";
}