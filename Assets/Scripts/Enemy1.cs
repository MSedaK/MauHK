using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : MonoBehaviour
{
    public int health = 100;
    public GameObject projectilePrefab;
    public GameObject hitEffect;
    public Transform firePoint;
    public float attackRange = 10f;
    public float fireRate = 2f;
    private float nextFireTime = 0f;
    public float moveSpeed = 3f;

    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Attack();
            }
        }
    }

    void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 direction = (player.position - firePoint.position).normalized;
        rb.velocity = direction * 10f;

        Debug.Log(gameObject.name + " fired at the player!");
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

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
