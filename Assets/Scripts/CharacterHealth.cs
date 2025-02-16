using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public int health = 100;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager'ý buluyoruz
    }

    // Karakterin aldýðý hasarý hesapla
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        if (health <= 0)
        {
            Die();  
        }
    }

    // Karakterin ölmesini saðla
    void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");
        gameManager.GameOver(); // GameManager'a bildiriyoruz
        gameObject.SetActive(false); // Karakteri devre dýþý býrakýyoruz
    }

    // Karakterin ölmediðini kontrol et
    public bool IsDead()
    {
        return health <= 0;
    }
}
