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
    public TMP_Text gameOverScoreText; // Skoru GameOver paneline yazdýran TextMeshPro referansý
    public GameObject restartButton; // Restart butonu

    [Header("Enemy Spawn Settings")]
    public Transform[] spawnPoints; // Düþman spawn noktalarý
    public GameObject[] enemyPrefabs; // Düþman prefablarý
    public int enemiesPerSpawn = 3; // Her spawn noktasýnda spawn olacak düþman sayýsý
    private float spawnDelay = 2f; // Spawn aralýðý
    private float nextSpawnTime = 0f;

    private int score = 0; // Skor

    void Start()
    {
        activeCharacter = characterA;
        characterA.SetActive(true);
        characterB.SetActive(false);

        gameOverPanel.SetActive(false); // GameOver paneli baþlangýçta gizli olacak

        // Ýlk düþman spawn'ýný baþlatýyoruz
        nextSpawnTime = Time.time + spawnDelay;
    }

    void Update()
    {
        // Karakterlerin ölümünü kontrol et
        if (characterA.GetComponent<CharacterHealth>().IsDead() || characterB.GetComponent<CharacterHealth>().IsDead())
        {
            GameOver();
        }

        // Düþman spawn iþlemi
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + spawnDelay; // Spawn gecikmesini ayarlýyoruz
        }
    }

    // Düþmanlarý spawn etme
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

    // Oyun bittiðinde Game Over panelini göster
    public void GameOver()
    {
        Time.timeScale = 0f; // Oyunu duraklatýyoruz
        gameOverPanel.SetActive(true); // Game Over panelini gösteriyoruz
        gameOverScoreText.text = "Final Score: " + score; // Skoru Game Over panelinde gösteriyoruz
        restartButton.SetActive(true); // Restart butonunu aktif hale getiriyoruz
    }

    // Oyunu yeniden baþlat
    public void RestartGame()
    {
        Time.timeScale = 1f; // Oyunu baþlatýyoruz
        score = 0; // Skoru sýfýrlýyoruz
        gameOverPanel.SetActive(false); // Game Over panelini kapatýyoruz

        // Baþlangýçta her iki karakteri de aktif hale getirin
        characterA.SetActive(true);
        characterB.SetActive(false);
        activeCharacter = characterA;

        // Karakterlerin saðlýklarýný sýfýrlayalým
        characterA.GetComponent<CharacterHealth>().health = 100;
        characterB.GetComponent<CharacterHealth>().health = 100;

        // Tüm düþmanlarý temizleyelim
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    // Skoru güncelle
    public void UpdateScore(int points)
    {
        score += points;
    }

    // EnemyKilled fonksiyonunu public yapýyoruz
    public void EnemyKilled(int damage)
    {
        int killBonus = 10; // Ekstra öldürme bonusu ekleyebilirsiniz

        // Skor hesaplama
        score += damage + killBonus;
        Debug.Log("Score: " + score);

        // Skorun UI'ye yansýmasý
        UpdateScoreUI();
    }

    // Skoru UI'ye yansýtma
    private void UpdateScoreUI()
    {
        // Burada skoru ekrana yazdýrabilirsiniz.
        Debug.Log("Updated Score: " + score);
    }
}
