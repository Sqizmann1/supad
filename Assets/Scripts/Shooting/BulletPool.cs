using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int poolSize = 30;

    private Queue<Bullet> pool = new Queue<Bullet>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
            CreateBullet();
    }

    Bullet CreateBullet()
    {
        var b = Instantiate(bulletPrefab, transform);
        b.gameObject.SetActive(false);
        pool.Enqueue(b);
        return b;
    }

    public Bullet Get(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
            CreateBullet();

        var bullet = pool.Dequeue();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);

        // Возврат в пул через lifetime
        StartCoroutine(ReturnAfterDelay(bullet, bullet.lifetime + 0.1f));
        return bullet;
    }

    System.Collections.IEnumerator ReturnAfterDelay(Bullet b, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!b.gameObject.activeSelf) // уже деактивирована — просто вернуть
            pool.Enqueue(b);
    }

    // Bullet сама себя деактивирует → вызови Return явно
    public void Return(Bullet b)
    {
        b.gameObject.SetActive(false);
        pool.Enqueue(b);
    }
}