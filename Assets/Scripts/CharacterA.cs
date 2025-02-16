using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterA : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public int baseAttackDamage = 20;
    public int extraDamageToPreferredEnemies = 10;
    public int extraDamageFromWeaknessEnemies = 5;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private Animator anim;
    private int enemyKillCount = 0;
    public int upgradeThreshold = 10;
    public int upgradeDamageBoost = 5;
    public string[] preferredEnemies;
    public string[] weaknessEnemies;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    private Rigidbody rb;

    // Bufflar için gerekli olan deðiþkenler
    public float moveSpeed = 5f; // Baþlangýçta hareket hýzý
    public float staminaRegenRate = 10f; // Baþlangýçta stamina dolum hýzý

    [Header("UI Panels")]
    public GameObject panelC;
    public GameObject panelD;

    private bool canvasOpened = false;
    private HashSet<string> triggeredZones = new HashSet<string>();

    // Buff Deðiþkenleri
    private float originalMoveSpeed;
    private float originalDashRange;
    private float originalStaminaRegenRate;
    private float originalDashSpeed;

    private float dashRangeIncrease = 2f; // Dash menzilini artýrmak için
    private float staminaRegenIncrease = 1.15f; // %15 Stamina fill speed
    private float dashSpeedIncrease = 1.30f; // %30 Dash speed
    private float moveSpeedIncrease = 1.10f; // %10 Fast move

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Baþlangýç deðerlerini sakla
        originalMoveSpeed = moveSpeed;
        originalDashRange = dashSpeed;
        originalStaminaRegenRate = staminaRegenRate;
        originalDashSpeed = dashSpeed;

        if (panelC != null)
            panelC.SetActive(false);
        if (panelD != null)
            panelD.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canvasOpened)
        {
            if (other.CompareTag("TriggerZoneC") && !triggeredZones.Contains(other.gameObject.name))
            {
                triggeredZones.Add(other.gameObject.name);
                OpenCanvas(panelC);
            }
            else if (other.CompareTag("TriggerZoneD") && !triggeredZones.Contains(other.gameObject.name))
            {
                triggeredZones.Add(other.gameObject.name);
                OpenCanvas(panelD);
            }
        }
    }

    void OpenCanvas(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            canvasOpened = true;
        }
    }

    // CloseCanvas fonksiyonunu parametre alacak þekilde tanýmlayýn
    public void CloseCanvas(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        canvasOpened = false;
    }



    public void SelectBuffC(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 1: // %10 Fast Move
                moveSpeed = originalMoveSpeed * 1.10f;
                break;

            case 2: // %30 Fast Move
                moveSpeed = originalMoveSpeed * 1.30f;
                break;

            case 3: // Longer Dash Range
                dashSpeed = originalDashSpeed * 1.5f; // Dash menzilini %50 artýr
                break;
        }

        CloseCanvas(panelC); // Panel C'yi kapat
    }

    public void SelectBuffD(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 1: // %15 Stamina Fill Speed
                staminaRegenRate = originalStaminaRegenRate * 1.15f;
                break;

            case 2: // %30 Dash Speed
                dashSpeed = originalDashSpeed * 1.30f; // Dash hýzýný %30 artýr
                break;

            case 3: // Longer Dash Range
                dashSpeed = originalDashSpeed * 1.5f; // Dash menzilini %50 artýr
                break;
        }

        CloseCanvas(panelD); // Panel D'yi kapat
    }


    IEnumerator Attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.2f);

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * attackRange, 1f, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            HandleEnemyDamage(enemy.gameObject, baseAttackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        Vector3 dashDirection = transform.forward;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            DealDashDamage();
            yield return null;
        }

        isDashing = false;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void DealDashDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 1.5f, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            HandleEnemyDamage(enemy.gameObject, 50);
        }
    }

    void HandleEnemyDamage(GameObject enemyObject, int damage)
    {
        Enemy1 enemy1 = enemyObject.GetComponent<Enemy1>();
        if (enemy1 != null)
        {
            enemy1.TakeDamage(damage);
            RegisterKill();
            return;
        }

        Enemy2 enemy2 = enemyObject.GetComponent<Enemy2>();
        if (enemy2 != null)
        {
            enemy2.TakeDamage(damage);
            RegisterKill();
        }
    }

    void RegisterKill()
    {
        enemyKillCount++;
        if (enemyKillCount >= upgradeThreshold)
        {
            UpgradeAttack();
            enemyKillCount = 0;
        }
    }

    void UpgradeAttack()
    {
        baseAttackDamage += upgradeDamageBoost;
        extraDamageToPreferredEnemies += upgradeDamageBoost;
        Debug.Log("Character A upgraded!");
    }
}
