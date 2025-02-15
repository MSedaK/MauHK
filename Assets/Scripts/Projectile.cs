using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f; // 5 saniye sonra yok ol

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Oyuncuya çarptýysa
        {
            CharacterB player = other.GetComponent<CharacterB>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject); // Mermiyi yok et
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Oyuncuya çarptýðýnda çalýþacak
        {
            CharacterB player = collision.gameObject.GetComponent<CharacterB>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
