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
        if (other.CompareTag("Player")) // Sadece oyuncuya �arpt���nda hasar ver
        {
            CharacterB player = other.GetComponent<CharacterB>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy")) // E�er ba�ka bir nesneye �arparsa yok ol (duvar gibi)
        {
            Destroy(gameObject);
        }
    }
}
