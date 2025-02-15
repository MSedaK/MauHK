using System.Collections;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public int health = 100;
    public GameObject projectilePrefab; // Fýrlatýlacak obje (örneðin bir ateþ topu)
    public Transform firePoint; // Merminin çýkýþ noktasý
    public float attackRange = 10f; // Saldýrý menzili
    public float fireRate = 2f; // Kaç saniyede bir ateþ edecek
    private float nextFireTime = 0f;
    public float projectileSpeed = 10f; // Fýrlatýlan þeyin hýzý

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Attack();
        }
    }

    void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 direction = (player.position - firePoint.position).normalized;
        rb.velocity = direction * projectileSpeed;

        Debug.Log(gameObject.name + " fired at the player!");
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");
        Destroy(gameObject);
    }
}
