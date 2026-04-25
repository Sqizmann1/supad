using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    float size;
    public float duration;
    public Transform zone;
    public float shrinkSpeed = 2f;
    public float minSize = 5f;
    public Vector3 targetPosition;
    public float moveSpeed = 2f;

    private void Start()
    {
        float x = Random.Range(20, 200);
        float z = Random.Range(20, 200);
        targetPosition = new Vector3(x, 15, z);
    }

    void Update()
    {
        if (zone.localScale.x > minSize)
        {
            zone.localScale -= new Vector3(1, 0, 1) * shrinkSpeed * Time.deltaTime;
        }
        zone.position = Vector3.MoveTowards(zone.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}