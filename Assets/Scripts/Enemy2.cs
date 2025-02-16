using UnityEngine.AI;
using UnityEngine;

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
    private GameManager gameManager; // GameManager referansý ekliyoruz

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        gameManager = FindObjectOfType<GameManager>(); // GameManager'ý buluyoruz
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
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                Attack();
            }
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

    // TakeDamage fonksiyonunu public yapýyoruz
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        if (health <= 0)
        {
            Die(damage); // Ölünce GameManager'a damage'ý bildiriyoruz
        }
    }

    void Die(int damage)
    {
        Debug.Log(gameObject.name + " has been defeated!");
        gameManager.EnemyKilled(damage); // GameManager'a damage parametresi gönderiyoruz
        Destroy(gameObject);
    }
}
