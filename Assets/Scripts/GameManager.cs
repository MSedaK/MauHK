using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Character Settings")]
    public GameObject characterA;
    public GameObject characterB;
    private GameObject activeCharacter;
    private CameraFollow cameraFollow;

    [Header("VFX Settings")]
    public GameObject swapVFXPrefab;

    [Header("Music Settings")]
    public AudioClip backgroundMusic;
    private AudioSource audioSource;

    [Header("Enemy Spawn Settings")]
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public int enemiesPerSpawn = 3;
    private float spawnDelay = 2f;
    private float nextSpawnTime = 0f;

    [Header("Score Settings")]
    public int score = 0;

    [Header("Health Settings")]
    public int maxHealth = 10;
    private int health;

    // UI Referanslarý
    public TMP_Text scoreText;
    public TMP_Text healthText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    public GameObject restartButton;

    private bool canvasOpened = false;

    void Start()
    {
        activeCharacter = characterA;
        characterA.SetActive(true);
        characterB.SetActive(false);

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.SetTarget(activeCharacter.transform);

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        health = maxHealth;
        UpdateUI();
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + spawnDelay;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SwapCharacter();
        }
    }

    void SwapCharacter()
    {
        if (swapVFXPrefab != null)
        {
            Instantiate(swapVFXPrefab, activeCharacter.transform.position, Quaternion.identity);
        }

        Vector3 currentPosition = activeCharacter.transform.position;

        activeCharacter.SetActive(false);
        activeCharacter = (activeCharacter == characterA) ? characterB : characterA;
        activeCharacter.transform.position = currentPosition;
        activeCharacter.SetActive(true);

        cameraFollow.SetTarget(activeCharacter.transform);
    }

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

    public void EnemyKilled(int damage)
    {
        score += damage;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateUI();

        if (damage >= 2 || health <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (healthText != null)
        {
            healthText.text = "Health: " + health;
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Final Score: " + score;
        restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        score = 0;
        health = maxHealth;
        UpdateUI();
        gameOverPanel.SetActive(false);
        characterA.SetActive(true);
        characterB.SetActive(false);
        activeCharacter = characterA;

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    void OnDestroy()
    {
        if (gameObject.CompareTag("Player"))
        {
            GameOver();
        }
    }
}
