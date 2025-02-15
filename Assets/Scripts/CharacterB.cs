using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterB : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject infoPanel; // UI Panel (Inspector'dan atayacaks�n)
    private HashSet<string> triggeredZones = new HashSet<string>(); // Hangi alanlara girildi�ini takip eder

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
    public int damageTaken = 20; // D��mandan al�nan hasar
    public Slider healthBar;
    public Image[] healthSegments; 

    private Vector3 lastMouseDirection = Vector3.zero; // Son mouse y�n�n� sakl�yoruz

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        anim = GetComponent<Animator>();
        stamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = stamina;
        rb = GetComponent<Rigidbody>();

        if (infoPanel != null)
            infoPanel.SetActive(false); // UI ba�lang��ta kapal� olacak
    }

    void Update()
    {
        RotateTowardsMouse(); // Karakteri mouse y�n�ne d�nd�r
        HandleMovement(); // WASD ile hareket et

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

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (lastMouseDirection.magnitude >= 0.1f) // Mouse hareket ettiyse y�n belirle
        {
            Vector3 forwardDirection = lastMouseDirection; // Mouse'un y�n�ne g�re hareket et
            moveDirection = (forwardDirection * moveZ + Vector3.Cross(Vector3.up, forwardDirection) * moveX).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
            }
        }
    }

    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; // Y eksenini sabit tut

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction.magnitude > 0.1f)
            {
                lastMouseDirection = direction; // Mouse y�n�n� kaydet
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 15f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerZone")) // E�er bir "TriggerZone" i�ine girdiysek
        {
            if (!triggeredZones.Contains(other.gameObject.name)) // Daha �nce girilmediyse
            {
                triggeredZones.Add(other.gameObject.name); // Kaydet
                StartCoroutine(ShowUIPanel()); // UI Panel a�
            }
        }

        if (other.CompareTag("Enemy")) // Enemy'e temas ederse
        {
            TakeDamage(damageTaken);
        }
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
        healthBar.value = healthPercentage; // Slider g�ncelle

        int activeSegments = Mathf.RoundToInt(healthPercentage * healthSegments.Length);
        for (int i = 0; i < healthSegments.Length; i++)
        {
            healthSegments[i].enabled = i < activeSegments; // Par�al� bar g�ncelle
        }
    }

    void Die()
    {
        Debug.Log("Karakter �ld�!");
        gameObject.SetActive(false); // �imdilik karakteri yok edelim
    }

    IEnumerator ShowUIPanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
            yield return new WaitForSeconds(3f); // 3 saniye sonra kapanacak
            infoPanel.SetActive(false);
        }
    }

    IEnumerator Attack(GameObject sphereType, float speed)
    {
        canAttack = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.2f);

        GameObject bomb = Instantiate(sphereType, firePoint.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        bomb.tag = "Bomb"; // Bomb tag'ini at�yoruz
        Bomb bombScript = bomb.AddComponent<Bomb>(); // Bomb scripti ekle
        bombScript.damage = (sphereType == spherePrefab) ? 10 : 20; // E�er Strong ise 20 hasar

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = (hit.point - firePoint.position).normalized;
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = firePoint.forward * speed;
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator StrongAttack()
    {
        canAttack = false;
        stamina -= strongAttackCost;
        staminaBar.value = stamina;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.2f);

        GameObject bomb = Instantiate(strongSpherePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        bomb.tag = "Bomb"; // Bomb tag'ini at�yoruz
        Bomb bombScript = bomb.AddComponent<Bomb>(); // Bomb scripti ekle
        bombScript.damage = 20; // G��l� bomban�n hasar� daha fazla

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = (hit.point - firePoint.position).normalized;
            rb.velocity = direction * strongSphereSpeed;
        }
        else
        {
            rb.velocity = firePoint.forward * strongSphereSpeed;
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
