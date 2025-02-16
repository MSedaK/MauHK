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

    private Vector3 lastMouseDirection = Vector3.zero;

    private bool canvasOpened = false; // To track if the canvas has been opened

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
            infoPanel.SetActive(false);
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
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (lastMouseDirection.magnitude >= 0.1f)
        {
            Vector3 forwardDirection = lastMouseDirection;
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
        // TriggerZoneB'ye girdiğinde Canvas açılır
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

    // Canvas açma işlemi
    void OpenCanvas()
    {
        infoPanel.SetActive(true);
        canvasOpened = true; // Canvas açıldı
    }

    // Canvas kapama işlemi
    public void CloseCanvas()
    {
        infoPanel.SetActive(false);
        canvasOpened = false; // Canvas kapandı
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
        anim.SetTrigger("Attack");

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

        bomb.tag = "Bomb";
        Bomb bombScript = bomb.AddComponent<Bomb>();
        bombScript.damage = 20;

        Vector3 shootDirection = GetShootDirection();
        rb.velocity = shootDirection * (strongSphereSpeed * 1.5f);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
