using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterB : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject infoPanelA; // TriggerZoneA için panel
    public GameObject infoPanelB; // TriggerZoneB için panel
    private HashSet<string> triggeredZones = new HashSet<string>();

    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Saldırı Ayarları")]
    public float attackRange = 2.0f;
    public int baseAttackDamage = 15;
    public float attackCooldown = 0.7f;
    private bool canAttack = true;
    private Animator anim;

    [Header("Sphere Saldırı Ayarları")]
    public GameObject spherePrefab;
    public GameObject strongSpherePrefab;
    public Transform firePoint;
    public float sphereSpeed = 10f;
    public float strongSphereSpeed = 8f;
    private float originalSphereSpeed;

    [Header("Stamina Ayarları")]
    public float maxStamina = 100f;
    public float stamina;
    public float strongAttackCost = 50f;
    public float staminaRegenRate = 10f;
    public Slider staminaBar;

    [Header("Sağlık Ayarları")]
    public int maxHealth = 100;
    public int currentHealth;
    public int damageTaken = 20;
    public Slider healthBar;
    public Image[] healthSegments;

    [Header("Ses Ayarları")]
    public AudioClip attackSound;
    public AudioClip strongAttackSound;
    private AudioSource audioSource;

    // Başlangıç değerleri
    private float originalStaminaRegenRate;
    private float originalDashCooldown;
    private float originalMoveSpeed;

    private Vector3 lastMouseDirection = Vector3.zero;

    private bool canvasOpened = false;
    private bool isBuffActive = false;

    private float sphereBuffDuration = 10f; 
    private bool canFireMultipleSpheres = false; 

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

        originalSphereSpeed = sphereSpeed;
        originalStaminaRegenRate = staminaRegenRate; 
        originalDashCooldown = attackCooldown;  
        originalMoveSpeed = moveSpeed;  

        if (infoPanelA != null)
            infoPanelA.SetActive(false);
        if (infoPanelB != null)
            infoPanelB.SetActive(false);
    }


    void Update()
    {
        RotateTowardsMouse();
        HandleMovement();

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack(spherePrefab, sphereSpeed));
        }
        else if (Input.GetMouseButtonDown(1) && canAttack && stamina >= strongAttackCost)
        {
            StartCoroutine(StrongAttack());
        }

        RegenerateStamina();

        if (Input.GetKeyDown(KeyCode.P)) 
        {
            SwitchCharacter();
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        if (moveDirection.magnitude >= 0.1f)
        {
            rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
            anim.SetFloat("Speed", rb.velocity.magnitude);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }

    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction.magnitude > 0.1f)
            {
                lastMouseDirection = direction;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerZoneA") && !triggeredZones.Contains("TriggerZoneA"))
        {
            triggeredZones.Add("TriggerZoneA");
            OpenCanvas(infoPanelA); // TriggerZoneA için paneli aç
        }

        if (other.CompareTag("TriggerZoneB") && !triggeredZones.Contains("TriggerZoneB"))
        {
            triggeredZones.Add("TriggerZoneB");
            OpenCanvas(infoPanelB); // TriggerZoneB için paneli aç
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(damageTaken);
        }
    }

    void OpenCanvas(GameObject panel)
    {
        panel.SetActive(true);
        canvasOpened = true;
        Time.timeScale = 0f; // Oyunu duraklat
    }

    public void CloseCanvas(GameObject panel)
    {
        panel.SetActive(false);
        canvasOpened = false;
        Time.timeScale = 1f; // Oyunu devam ettir
    }

    public void SelectBuff(int buttonIndex)
    {
        // Buton seçimine göre buff uygulaması yapılır
        switch (buttonIndex)
        {
            case 1: // Buton 1: %15 Sphere hızını artır
                sphereSpeed = originalSphereSpeed * 1.15f;
                break;

            case 2: // Buton 2: %30 Sphere hızını artır (10 saniye)
                StartCoroutine(SphereSpeedBuff(0.3f, sphereBuffDuration));
                break;

            case 3: // Buton 3: Karakter değişene kadar 2 sphere atabilme
                canFireMultipleSpheres = true;
                break;

            case 4: // Buton 4: %15 Stamina Fill Speed
                staminaRegenRate = originalStaminaRegenRate * 1.15f;
                break;

            case 5: // Buton 5: %15 Less Dash Cooldown
                attackCooldown = originalDashCooldown * 0.85f;  // Dash cooldown'u %15 azalt
                break;

            case 6: // Buton 6: %20 Fast Move
                moveSpeed = originalMoveSpeed * 1.20f;  // Hareket hızını %20 artır
                break;
        }

        // Seçim yapıldıktan sonra paneli kapat
        CloseCanvas(infoPanelA);
        CloseCanvas(infoPanelB);
    }




    IEnumerator SphereSpeedBuff(float percentage, float duration)
    {
        float originalSpeed = sphereSpeed;
        sphereSpeed += originalSpeed * percentage;

        yield return new WaitForSeconds(duration);

        sphereSpeed = originalSpeed; // Süre bitince hız eski haline döner
    }

    public void SwitchCharacter()
    {
        // Karakter değiştiğinde buff'ları sıfırla
        sphereSpeed = originalSphereSpeed;
        canFireMultipleSpheres = false;
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

        // Saldırı sesi
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

        // Güçlü saldırı sesi
        if (audioSource != null && strongAttackSound != null)
        {
            audioSource.PlayOneShot(strongAttackSound);
        }

        yield return new WaitForSeconds(0.2f);

        // Fırlatma Yönlerini Belirleyelim (İleri, Sola, Sağa)
        Vector3 forwardDirection = transform.forward; // İleri
        Vector3 rightDirection = transform.right; // Sağa
        Vector3 leftDirection = -transform.right; // Sola

        // Sphere'leri fırlatalım
        FireStrongSphere(forwardDirection);
        FireStrongSphere(rightDirection);
        FireStrongSphere(leftDirection);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        anim.SetBool("IsStrongAttacking", false);
    }

    void FireStrongSphere(Vector3 direction)
    {
        // Sphere prefab'ını instantiate edelim
        GameObject bomb = Instantiate(strongSpherePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        bomb.tag = "Bomb";
        Bomb bombScript = bomb.AddComponent<Bomb>();
        bombScript.damage = 20;

        // Fırlatma hızını belirleyelim
        rb.velocity = direction * (strongSphereSpeed * 1.5f);
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
