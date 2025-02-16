using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Character Settings")]
    public GameObject characterA;
    public GameObject characterB;
    private GameObject activeCharacter;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel; // GameOver paneli
    public TMP_Text gameOverScoreText; // Skoru GameOver paneline yazd�ran TextMeshPro referans�
    public GameObject restartButton; // Restart butonu

    [Header("Enemy Spawn Settings")]
    public Transform[] spawnPoints; // D��man spawn noktalar�
    public GameObject[] enemyPrefabs; // D��man prefablar�
    public int enemiesPerSpawn = 3; // Her spawn noktas�nda spawn olacak d��man say�s�
    private float spawnDelay = 2f; // Spawn aral���
    private float nextSpawnTime = 0f;

    private int score = 0; // Skor

    void Start()
    {
        activeCharacter = characterA;
        characterA.SetActive(true);
        characterB.SetActive(false);

        gameOverPanel.SetActive(false); // GameOver paneli ba�lang��ta gizli olacak

        // �lk d��man spawn'�n� ba�lat�yoruz
        nextSpawnTime = Time.time + spawnDelay;
    }

    void Update()
    {
        // Karakterlerin �l�m�n� kontrol et
        if (characterA.GetComponent<CharacterHealth>().IsDead() || characterB.GetComponent<CharacterHealth>().IsDead())
        {
            GameOver();
        }

        // D��man spawn i�lemi
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + spawnDelay; // Spawn gecikmesini ayarl�yoruz
        }
    }

    // D��manlar� spawn etme
    void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                GameObject selectedEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Instantiate(selectedEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }

    // Oyun bitti�inde Game Over panelini g�ster
    public void GameOver()
    {
        Time.timeScale = 0f; // Oyunu duraklat�yoruz
        gameOverPanel.SetActive(true); // Game Over panelini g�steriyoruz
        gameOverScoreText.text = "Final Score: " + score; // Skoru Game Over panelinde g�steriyoruz
        restartButton.SetActive(true); // Restart butonunu aktif hale getiriyoruz
    }

    // Oyunu yeniden ba�lat
    public void RestartGame()
    {
        Time.timeScale = 1f; // Oyunu ba�lat�yoruz
        score = 0; // Skoru s�f�rl�yoruz
        gameOverPanel.SetActive(false); // Game Over panelini kapat�yoruz

        // Ba�lang��ta her iki karakteri de aktif hale getirin
        characterA.SetActive(true);
        characterB.SetActive(false);
        activeCharacter = characterA;

        // Karakterlerin sa�l�klar�n� s�f�rlayal�m
        characterA.GetComponent<CharacterHealth>().health = 100;
        characterB.GetComponent<CharacterHealth>().health = 100;

        // T�m d��manlar� temizleyelim
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    // Skoru g�ncelle
    public void UpdateScore(int points)
    {
        score += points;
    }

    // EnemyKilled fonksiyonunu public yap�yoruz
    public void EnemyKilled(int damage)
    {
        int killBonus = 10; // Ekstra �ld�rme bonusu ekleyebilirsiniz

        // Skor hesaplama
        score += damage + killBonus;
        Debug.Log("Score: " + score);

        // Skorun UI'ye yans�mas�
        UpdateScoreUI();
    }

    // Skoru UI'ye yans�tma
    private void UpdateScoreUI()
    {
        // Burada skoru ekrana yazd�rabilirsiniz.
        Debug.Log("Updated Score: " + score);
    }
}
