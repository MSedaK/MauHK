using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public GameObject hitEffect;
    private GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();  // GameManager'ý buluyoruz
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
        //gameManager.EnemyKilled();
        Debug.Log(gameObject.name + " has been defeated!");
        Destroy(gameObject);
    }
}