using System.Collections;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public int health = 100;
    public float attackRange = 2f; // Yakýn dövüþ mesafesi
    public float attackCooldown = 1.5f; // Kaç saniyede bir saldýracak
    private float nextAttackTime = 0f;
    public int damage = 15; // Oyuncuya vereceði hasar

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log(gameObject.name + " attacked the player!");

        CharacterB playerScript = player.GetComponent<CharacterB>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
        }
    }

    public void TakeDamage( int damage)
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
