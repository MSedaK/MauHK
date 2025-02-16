using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public int health = 100;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager'� buluyoruz
    }

    // Karakterin ald��� hasar� hesapla
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        if (health <= 0)
        {
            Die();  
        }
    }

    // Karakterin �lmesini sa�la
    void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");
        gameManager.GameOver(); // GameManager'a bildiriyoruz
        gameObject.SetActive(false); // Karakteri devre d��� b�rak�yoruz
    }

    // Karakterin �lmedi�ini kontrol et
    public bool IsDead()
    {
        return health <= 0;
    }
}
