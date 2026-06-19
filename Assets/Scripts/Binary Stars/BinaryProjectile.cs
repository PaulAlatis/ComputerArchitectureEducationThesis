using UnityEngine;

public class BinaryProjectile : MonoBehaviour
{
    [SerializeField] int bulletValue; // 0 or 1

    private void OnTriggerEnter2D(Collider2D other)
    {
        BinaryOrb orb = other.GetComponent<BinaryOrb>();

        if (orb != null)
        {
            orb.CheckShot(bulletValue);
            Destroy(gameObject);
        }
    }
}