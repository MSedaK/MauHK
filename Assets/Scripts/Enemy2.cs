using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // UI i�in eklenmesi gerekli

public class Enemy2 : MonoBehaviour
{
    public int health = 100;
    public float attackRange = 2f; 
    public float attackCooldown = 1.5f; 
    private float nextAttackTime = 0f;
    public int damage = 15; 
    public float moveSpeed = 3.5f; 

    private Transform player;
    private NavMeshAgent agent;

    // Health Bar UI
    public Slider healthBar; // Health bar referans�
    private GameObject healthBarObject; // Health bar'�n prefab'�n� instantiate edece�iz

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; 

        // Health Bar prefab'�n� instantiate ediyoruz
        healthBarObject = Instantiate(healthBar.gameObject, transform.position + Vector3.up, Quaternion.identity);
        healthBarObject.transform.SetParent(Camera.main.transform); // UI'yi kamera ile ba�lar�z
        healthBarObject.transform.localPosition = new Vector3(0, 0, 0); // Sa�l�k �ubu�unu do�ru konumda tutmak i�in ayarl�yoruz

        // Health bar'�n maksimum de�eri, d��man�n sa�l�k de�eri
        healthBar.maxValue = health;
        healthBar.value = health;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Enemy hareketi
        if (distance > attackRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                Attack();
            }
        }

        // Health bar'� her g�ncellenen sa�l�k durumuna g�re g�ncelle
        if (healthBarObject != null)
        {
            healthBar.value = health; // Health �ubu�unu g�ncelle
            healthBarObject.transform.position = transform.position + Vector3.up; // Sa�l�k �ubu�unu d��man�n �st�nde tutmak i�in
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            CharacterB playerScript = collision.gameObject.GetComponent<CharacterB>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        // D��man �ld�yse
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");
        Destroy(gameObject);
        if (healthBarObject != null)
        {
            Destroy(healthBarObject); // D��man �ld���nde health bar'� da yok et
        }
    }
}
