using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int damage = 10;
    public float explosionRadius = 1.5f; // Patlama yar��ap�
    public GameObject explosionEffect; // Patlama efekti (varsa)

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // E�er d��mana �arparsa
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // D��mana hasar ver
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity); // Patlama efekti
            }

            Destroy(gameObject); // �arp�nca yok ol
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // E�er d��mana �arparsa
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // D��mana hasar ver
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity); // Patlama efekti
            }

            Destroy(gameObject); // �arp�nca yok ol
 �������}
    }
}