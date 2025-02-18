using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterB : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject infoPanel;
    private HashSet<string> triggeredZones = new HashSet<string>();

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Attack Settings")]
    public float attackRange = 2.0f;
    public int baseAttackDamage = 15;
    public float attackCooldown = 0.7f;
    private bool canAttack = true;
    private Animator anim;

    [Header("Sphere Attack Settings")]
    public GameObject spherePrefab;
    public GameObject strongSpherePrefab;
    public Transform firePoint;
    public float sphereSpeed = 10f;
    public float strongSphereSpeed = 8f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float stamina;
    public float strongAttackCost = 50f;
    public float staminaRegenRate = 10f;
    public Slider staminaBar;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public int damageTaken = 20;
    public Slider healthBar;
    public Image[] healthSegments;

    [Header("Audio Settings")]
    public AudioClip attackSound; 
    public AudioClip strongAttackSound; 
    private AudioSource audioSource; 

    private Vector3 lastMouseDirection = Vector3.zero;

    private bool canvasOpened = false; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        anim = GetComponent<Animator>();
        stamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = stamina;
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>(); 

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack(spherePrefab, sphereSpeed));
        }
        else if (Input.GetMouseButtonDown(1) && canAttack && stamina >= strongAttackCost)
        {
            StartCoroutine(StrongAttack());
        }

        RegenerateStamina();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerZoneB") && !canvasOpened)
        {
            if (!triggeredZones.Contains(other.gameObject.name))
            {
                triggeredZones.Add(other.gameObject.name);
                OpenCanvas();
            }
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(damageTaken);
        }
    }

    void OpenCanvas()
    {
        infoPanel.SetActive(true);
        canvasOpened = true; 
    }

    public void CloseCanvas()
    {
        infoPanel.SetActive(false);
        canvasOpened = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        healthBar.value = healthPercentage;

        int activeSegments = Mathf.RoundToInt(healthPercentage * healthSegments.Length);
        for (int i = 0; i < healthSegments.Length; i++)
        {
            healthSegments[i].enabled = i < activeSegments;
        }
    }

    void Die()
    {
        Debug.Log("Karakter öldü!");
        gameObject.SetActive(false);
    }

    IEnumerator Attack(GameObject sphereType, float speed)
    {
        canAttack = false;
        anim.SetBool("IsAttacking", true); 

        // Play attack sound
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        yield return new WaitForSeconds(0.1f);

        GameObject bomb = Instantiate(sphereType, firePoint.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        bomb.tag = "Bomb";
        Bomb bombScript = bomb.AddComponent<Bomb>();
        bombScript.damage = (sphereType == spherePrefab) ? 10 : 20;

        Vector3 shootDirection = GetShootDirection();
        rb.velocity = shootDirection * (speed * 2f);

        yield return new WaitForSeconds(attackCooldown / 2f);
        canAttack = true;
        anim.SetBool("IsAttacking", false); 
    }

    IEnumerator StrongAttack()
    {
        canAttack = false;
        stamina -= strongAttackCost;
        staminaBar.value = stamina;
        anim.SetBool("IsStrongAttacking", true); 

        // Play strong attack sound
        if (audioSource != null && strongAttackSound != null)
        {
            audioSource.PlayOneShot(strongAttackSound);
        }

        yield return new WaitForSeconds(0.2f);

        GameObject bomb = Instantiate(strongSpherePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        bomb.tag = "Bomb";
        Bomb bombScript = bomb.AddComponent<Bomb>();
        bombScript.damage = 20;

        Vector3 shootDirection = GetShootDirection();
        rb.velocity = shootDirection * (strongSphereSpeed * 1.5f);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        anim.SetBool("IsStrongAttacking", false); 
    }

    Vector3 GetShootDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = (hit.point - firePoint.position).normalized;
            lastMouseDirection = direction;
            return direction;
        }

        return lastMouseDirection;
    }

    void RegenerateStamina()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            staminaBar.value = stamina;
        }
    }
}
