using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int damage = 10;
    public float explosionRadius = 1.5f; // Patlama yarýçapý
    public GameObject explosionEffect; // Patlama efekti (varsa)

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Eðer düþmana çarparsa
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Düþmana hasar ver
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity); // Patlama efekti
            }

            Destroy(gameObject); // Çarpýnca yok ol
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Eðer düþmana çarparsa
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Düþmana hasar ver
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity); // Patlama efekti
            }

            Destroy(gameObject); // Çarpýnca yok ol
        }
    }
}